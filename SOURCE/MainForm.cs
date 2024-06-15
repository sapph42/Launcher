using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Launcher.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Management;
using System.Text.RegularExpressions;

namespace Launcher {
    public partial class MainForm : Form {
        //private Dictionary<string, string> buttons = new Dictionary<string, string>();

        private const int ButtonWidth = 94;
        private const int ButtonHeight = 52;
        private const int ButtonBuffer = 6;
        private const int FormStaticBufferWidth = 18;
        private const int FormStaticBufferHeight = 19;
        private const int MenuHeight = 25;
        private const int EditBuffer = 15;
        private ButtonCollection _buttons;
        private readonly EventHandler _click;
        private readonly MouseEventHandler _mouseDown;
        private readonly MouseEventHandler _mouseUp;
        private readonly DragEventHandler _dragEnter;
        private readonly DragEventHandler _dragDrop;
        private bool _rearrangeMode = false;
        private LauncherButton _dragSource;
        private readonly string _version;

        [DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        public static extern bool EnableWow64FSRedirection(bool enable);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled) {
            int useImmersiveDarkMode = enabled ? 1 : 0;
            return DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int)) == 0;
        }

        private class DarkRenderer : ToolStripProfessionalRenderer {
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                Color c = e.Item.Selected ? Color.Black : SystemColors.ControlDarkDark;
                using (SolidBrush brush = new SolidBrush(c))
                    e.Graphics.FillRectangle(brush, rc);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
                e.ArrowColor = Color.GhostWhite;
                base.OnRenderArrow(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
                int width = e.Item.Width;
                int height = e.Item.Height;
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                Color c = e.Item.Selected ? Color.Black : SystemColors.ControlDarkDark;
                using (SolidBrush brush = new SolidBrush(c)) {
                    e.Graphics.FillRectangle(brush, rc);
                    e.Graphics.DrawLine(new Pen(Color.GhostWhite), 4, height / 2, width - 4, height / 2);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
                e.TextColor = Color.GhostWhite;
                base.OnRenderItemText(e);
            }
        }

        public MainForm(string version) {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString = File.ReadAllText($@"{path}\launcher.json");
            _buttons = new ButtonCollection() {
                ButtonBuffer = ButtonBuffer,
                ButtonHeight = ButtonHeight,
                ButtonWidth = ButtonWidth,
                GridBufferHeight = MenuHeight + EditBuffer,
                GridBufferWidth = FormStaticBufferWidth
            };
            _click = delegate(object s, EventArgs e) {
                EnableWow64FSRedirection(false);
                LauncherButton button = (LauncherButton)s;
                if (string.IsNullOrEmpty(button.Caption))
                    return;
                Debug.WriteLine("Click event " + button.Text);
                if (string.IsNullOrEmpty(button.Path))
                    return;
                FileInfo target = new FileInfo(@"C:\");
                if (button.Path != "microsoft-edge:") {
                    target = new FileInfo(button.Path);
                    if (target.Extension == ".ps1") {
                        button.Arguments = $@"-File ""{button.Path}"" -ExecutionPolicy Bypass";
                        button.Path = "powershell.exe";
                    } else if (target.Extension == ".msc") {
                        button.Arguments = $@"""{button.Path}""";
                        button.Path = @"C:\Windows\System32\mmc.exe";
                    }
                }
                Process process;
                if (button.AdminOnly) {
                    process = new Process {
                        StartInfo = {
                            FileName = button.Path,
                            Arguments = button.Arguments,
                            UseShellExecute = true,
                            Verb = "runas"
                        }
                    };
                } else if (IsDirectory(button.Path)) {
                    process = new Process {
                        StartInfo = {
                            FileName = NormalizePath(button.Path),
                            UseShellExecute = true,
                            Verb = "open"
                        }
                    };
                } else if (button.Path == "microsoft-edge:") {
                    process = new Process{
                        StartInfo = {
                            FileName = $@"{button.Path}{button.Arguments}"
                        }
                    };
                } else {
                    process = new Process {
                        StartInfo = {
                            FileName = button.Path,
                            Arguments = button.Arguments,
                            UseShellExecute = target.Extension != ".exe",
                            Verb = ""
                        }
                    };
                }

                try {
                    process.Start();
                } catch (System.ComponentModel.Win32Exception) {
                    Debug.WriteLine("UAC Cancelled");
                }
                    
                EnableWow64FSRedirection(true);
            };
            _mouseDown = delegate(object s, MouseEventArgs e) {
                _dragSource = (LauncherButton)s;
                Debug.WriteLine("MouseDown event " + _dragSource.Text);
                _dragSource.DoDragDrop(_dragSource, DragDropEffects.Move);
            };
            _mouseUp = delegate(object s, MouseEventArgs e) {
                LauncherButton button = (LauncherButton)s;
                Debug.WriteLine("MouseUp event " + button.Text);
                _dragSource = null;
            };
            _dragEnter = delegate(object s, DragEventArgs e) {
                LauncherButton button = (LauncherButton)s;
                Debug.WriteLine("DragEnter event " + button.Text);
                if (
                    e.Data.GetDataPresent("Launcher.LauncherButton") 
                    && s != null
                    && _buttons.Any(b => b.Caption == ((LauncherButton)e.Data.GetData("Launcher.LauncherButton")).Caption)
                )
                    e.Effect = DragDropEffects.Move;
                else
                    e.Effect = DragDropEffects.None;
            };
            _dragDrop = delegate(object s, DragEventArgs e) {
                LauncherButton destinationButton = (LauncherButton)s;
                LauncherButton copyOfDestination = destinationButton.Clone();
                Debug.WriteLine("DragDrop event " + destinationButton.Text);

                destinationButton.Caption = _dragSource.Caption;
                destinationButton.Path = _dragSource.Path;
                destinationButton.Arguments = _dragSource.Arguments;
                destinationButton.Background = _dragSource.Background;
                destinationButton.BackColor = _dragSource.BackColor;
                destinationButton.ForeColor = _dragSource.ForeColor;
                destinationButton.ReferenceType = _dragSource.ReferenceType;

                _dragSource.Caption = copyOfDestination.Caption;
                _dragSource.Path = copyOfDestination.Path;
                _dragSource.Arguments = copyOfDestination.Arguments;
                _dragSource.Background = copyOfDestination.Background;
                _dragSource.BackColor = copyOfDestination.BackColor;
                _dragSource.ForeColor = copyOfDestination.ForeColor;
                _dragSource.ReferenceType = copyOfDestination.ReferenceType;
                _dragSource = null;
                RefreshCollection();
            };

            _buttons.AddRange(JsonConvert.DeserializeObject<HashSet<LauncherButton>>(jsonString));
            if (_buttons.Select(b => b.GridLocation).Count() >
                _buttons.Select(b => b.GridLocation).Distinct().Count()) {
                MessageBox.Show(
                    Resources.JSONError);
                foreach (LauncherButton button in _buttons)
                    button.GridLocation = new Point(0, 0);
                _buttons.Validate(true);
            } else 
                _buttons.Validate();
            InitializeComponent();
            if (Program.UsingDarkMode) {
                UseImmersiveDarkMode(Handle, true);
                Icon = Resources.white_rocket;
                menuStrip1.Renderer = new DarkRenderer();
                RightClickMenu.Renderer = new DarkRenderer();
                Invalidate();
            }
            DrawButtons();
            _version = version;
            Text = string.Format(Resources.DefaultMainTitle, _version);
        }

        public sealed override string Text {
            get => base.Text;
            set => base.Text = value;
        }

        private void DrawButtons(bool redraw = false) {
            SuspendLayout();
            if (redraw) {
                while (Controls.OfType<LauncherButton>().Any()) {
                    foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                        if (Controls.Find(button.Name, true).Last() is null)
                            continue;
                        Controls.Remove(Controls.Find(button.Name, true).Last());
                    }
                }
            }
            int titleBarHeight = Height - ClientRectangle.Height;
            Size = new Size() {
                Width = FormStaticBufferWidth + ButtonBuffer + ((ButtonBuffer + ButtonWidth) * _buttons.Width) + FormStaticBufferWidth + EditBuffer,
                Height = FormStaticBufferHeight + ButtonBuffer + ((ButtonBuffer + ButtonHeight) * _buttons.Height) + FormStaticBufferHeight + titleBarHeight + EditBuffer
            };
            foreach (LauncherButton button in _buttons) {
                button.Click += _click;
                button.MouseUp += _mouseUp;
                button.DragEnter += _dragEnter;
                button.DragDrop += _dragDrop;
                button.ContextMenuStrip = RightClickMenu;
                Controls.Add(button);
            }

            int buttonBottom = _buttons.Max(b => b.Location.Y) + ButtonHeight;
            int rightButton = _buttons.Max(b => b.Location.X) + ButtonWidth;
            AddRow.Location = new Point(AddRow.Location.X, buttonBottom + 3);
            RemoveRow.Location = new Point(RemoveRow.Location.X, buttonBottom - RemoveRow.Size.Height);
            AddColumn.Location = new Point(rightButton + 3, AddColumn.Location.Y);
            RemoveColumn.Location = new Point(rightButton - RemoveColumn.Size.Width, RemoveColumn.Location.Y);
            ResumeLayout(false);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            RefreshCollection();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString = JsonConvert.SerializeObject(Controls.OfType<LauncherButton>(), Formatting.Indented);
            File.WriteAllText($@"{path}\launcher.json", jsonString);
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            button.ReferenceType = LauncherButton.DetermineType(button);
            CreateOrEditButton createOrEditButton = new CreateOrEditButton(button);
            createOrEditButton.ShowDialog();
            if (createOrEditButton.DialogResult == DialogResult.Cancel)
                return;

            if (string.IsNullOrEmpty(createOrEditButton.Caption))
                return;
            Debug.WriteLine("Click event " + button.Text);
            if (string.IsNullOrEmpty(createOrEditButton.Path) && !IsDirectory(createOrEditButton.Arguments))
                return;
            if (createOrEditButton.ReferenceType != LauncherButton.RefType.Undetermined) {
                button.Path = createOrEditButton.Path;
                button.Caption = createOrEditButton.Caption;
                button.Arguments = createOrEditButton.Arguments;
                button.AdminOnly = createOrEditButton.AdminOnly;
                button.Background = createOrEditButton.Back;
                button.BackColor = button.Background;
                button.ReferenceType = createOrEditButton.ReferenceType;
                button.ColorCheck();
                return;
            }
            if (string.IsNullOrEmpty(createOrEditButton.Path) && IsDirectory(createOrEditButton.Arguments)) {
                createOrEditButton.Path = createOrEditButton.Arguments;
                createOrEditButton.Arguments = null;
            }
            try {
                Path.GetFullPath(createOrEditButton.Path);
            } catch {
                return;
            }
            try {
                createOrEditButton.Path = NormalizePath(createOrEditButton.Path);
                FileAttributes fileAttributes = File.GetAttributes(createOrEditButton.Path);
                if (fileAttributes.HasFlag(FileAttributes.Directory)) {
                    button.Path = createOrEditButton.Path;
                    button.Arguments = null;
                    button.ReferenceType = LauncherButton.RefType.Folder;
                } else {
                    FileInfo target = new FileInfo(createOrEditButton.Path);
                    if (target.Extension == ".ps1") {
                        button.Arguments = $@"-File ""{createOrEditButton.Path}"" -ExecutionPolicy Bypass";
                        button.Path = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
                        button.ReferenceType = LauncherButton.RefType.Powershell;
                    } else if (target.Extension == ".msc") {
                        button.Arguments = $@"""{createOrEditButton.Path}""";
                        button.Path = @"C:\Windows\System32\mmc.exe";
                    } else {
                        button.Path = createOrEditButton.Path;
                        button.Arguments = createOrEditButton.Arguments;
                    }
                }
                button.Caption = createOrEditButton.Caption;
                button.AdminOnly = createOrEditButton.AdminOnly;
                button.Background = createOrEditButton.Back;
                button.BackColor = button.Background;
            } catch {
                button.Caption = "";
                button.Path = "";
                button.Arguments = "";
                button.Background = button.DefaultBack;
                button.ColorCheck();
            }
            RefreshCollection();
        }

        private void RefreshCollection() {
            _buttons.Clear();
            _buttons
                .AddRange(
                    JsonConvert
                        .DeserializeObject<HashSet<LauncherButton>>(
                            JsonConvert
                                .SerializeObject(
                                    Controls.OfType<LauncherButton>(),
                                    Formatting.Indented
                                )
                        )
                );
        }
        private bool IsDirectory(string path) {
            if (path == "microsoft-edge:")
                return false;
            FileAttributes fileAttributes = File.GetAttributes(path);
            return fileAttributes.HasFlag(FileAttributes.Directory);
        }
        private string GetUncFromDriveLetter(string path) {
            if (path.First() == '\\' || Uri.IsWellFormedUriString(path, UriKind.Absolute))
                return path;
            string driveLetter = Directory.GetDirectoryRoot(path)
                .Replace(Path.DirectorySeparatorChar.ToString(), "");
            using (ManagementObject managementObject = new ManagementObject()) {
                managementObject.Path = new ManagementPath($@"\\{Environment.MachineName}\root\cimv2:Win32_LogicalDisk.DeviceID=""{driveLetter}""");
                return $@"{Convert.ToString(managementObject["ProviderName"])}\";
            }
        }
        private string NormalizePath(string path) {
            if (!Regex.IsMatch(path, @"\\\\.*"))
                path = path.Replace(Directory.GetDirectoryRoot(path), GetUncFromDriveLetter(path));
            if (IsDirectory(path) && path.Last() != Path.DirectorySeparatorChar)
                path += Path.DirectorySeparatorChar;
            return path;
        }
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            button.Caption = "";
            button.Path = "";
            button.Arguments = "";
            button.Background = button.DefaultBack;
            button.ColorCheck();
            RefreshCollection();
        }
        private void RunAsToolStripMenuItem_Click(object sender, EventArgs e) {
            EnableWow64FSRedirection(false);
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            if (string.IsNullOrEmpty(button.Caption))
                return;
            Debug.WriteLine("Click event " + button.Text);
            if (string.IsNullOrEmpty(button.Path))
                return;
            FileInfo target = new FileInfo(button.Path);
            if (target.Extension == ".ps1") {
                button.Arguments = $@"-File ""{button.Path}"" -ExecutionPolicy Bypass";
                button.Path = "powershell.exe";
            } else if (target.Extension == ".msc") {
                button.Arguments = $@"""{button.Path}""";
                button.Path = @"C:\Windows\System32\mmc.exe";
            }
            Process process = new Process {
                StartInfo = {
                    FileName = button.Path,
                    Arguments = button.Arguments,
                    UseShellExecute = true,
                    Verb = "runas"
                }
            };

            try {
                process.Start();
            } catch (System.ComponentModel.Win32Exception) {
                Debug.WriteLine("UAC Cancelled");
            }

            EnableWow64FSRedirection(true);
        }

        private void RearrangeMenu_Click(object sender, EventArgs e) {
            if (_rearrangeMode) {
                Text = string.Format(Resources.DefaultMainTitle, _version);
                foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                    button.MouseDown -= _mouseDown;
                }
            } else {
                Text = string.Format(Resources.RearrangeMainTitle, _version);
                foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                    button.MouseDown += _mouseDown;
                }
            }

            _rearrangeMode = !_rearrangeMode;
            RefreshCollection();
        }

        private void DoAddRow(object sender, EventArgs e) {
            _buttons.Add(new LauncherButton("", "", new Point(1, _buttons.Height + 1), ""));
            _buttons.Validate(true);
            DrawButtons(true);
        }

        private void DoAddColumn(object sender, EventArgs e) {
            _buttons.Add(new LauncherButton("", "", new Point(_buttons.Width + 1, 1), ""));
            _buttons.Validate();
            DrawButtons(true);
        }

        private void DoRemoveRow(object sender, EventArgs e) {
            int bottomRow = Controls.OfType<LauncherButton>().Max(b => b.GridLocation.Y);
            foreach (LauncherButton button in Controls.OfType<LauncherButton>()
                         .Where(b => b.GridLocation.Y == bottomRow)) {
                if (!string.IsNullOrEmpty(button.Caption))
                    return;
            }
            foreach (LauncherButton button in Controls.OfType<LauncherButton>()
                         .Where(b => b.GridLocation.Y == bottomRow)) {
                _buttons.Remove(button);
            }
            _buttons.Validate();
            DrawButtons(true);
        }

        private void DoRemoveColumn(object sender, EventArgs e) {
            int rightCol = Controls.OfType<LauncherButton>().Max(b => b.GridLocation.X);
            if (Controls.OfType<LauncherButton>().Where(b => b.GridLocation.X == rightCol)
                .Any(b => !string.IsNullOrEmpty(b.Caption)))
                return;
            _buttons.Remove(Controls.OfType<LauncherButton>()
                .Where(b => b.GridLocation.X == rightCol));
            _buttons.Validate();
            DrawButtons(true);
        }

        private void ExitMenu_Click(object sender, EventArgs e) {
            Close();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            if (Program.UsingDarkMode) {
                BackColor = SystemColors.ControlDarkDark;
                menuStrip1.BackColor = SystemColors.ControlDarkDark;
                menuStrip1.ForeColor = Color.GhostWhite;
                RightClickMenu.BackColor = SystemColors.ControlDarkDark;
                RightClickMenu.ForeColor = Color.GhostWhite;
                Invalidate();
            }
        }
    }
}
