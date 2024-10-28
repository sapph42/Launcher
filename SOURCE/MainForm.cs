using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Launcher.Properties;
using Newtonsoft.Json;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Launcher {
    public partial class MainForm : Form {
        private const int ButtonWidth = 94;
        private const int ButtonHeight = 52;
        private const int ButtonBuffer = 6;
        private const int FormStaticBufferWidth = 18;
        private const int FormStaticBufferHeight = 19;
        private const int MenuHeight = 25;
        private const int EditBuffer = 15;
        private ButtonCollection _buttons;
        private readonly EventHandler _click;
        private readonly System.Windows.Forms.MouseEventHandler _mouseDown;
        private readonly System.Windows.Forms.MouseEventHandler _mouseUp;
        private readonly DragEventHandler _dragEnter;
        private readonly DragEventHandler _dragDrop;
        private bool _rearrangeMode = false;
        private LauncherButton _dragSource;
        private readonly string _version;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private KeyboardHook hook = new KeyboardHook();

        [DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        public static extern bool EnableWow64FSRedirection(bool enable);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
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
                e.TextColor = e.Item.Enabled ? Color.GhostWhite : Color.LightGray;
                base.OnRenderItemText(e);
            }
        }
        private void ButtonClick(object s, EventArgs e) {
            EnableWow64FSRedirection(false);
            LauncherButton button = (LauncherButton)s;
            if (string.IsNullOrEmpty(button.Caption))
                return;
            Debug.WriteLine("Click event " + button.Text);
            if (string.IsNullOrEmpty(button.Path))
                return;
            FileInfo target = new FileInfo(@"C:\");
            string filename = "";
            string arguments = "";
            Process process;
            if (button.ReferenceType == LauncherButton.RefType.Undetermined)
                button.ReferenceType = LauncherButton.DetermineType(button);
            switch (button.ReferenceType) {
                case LauncherButton.RefType.Folder:
                    process = new Process {
                        StartInfo = {
                                    FileName = NormalizePath(button.Path),
                                    UseShellExecute = true,
                                    Verb = "open"
                                }
                    };
                    break;
                case LauncherButton.RefType.File:
                    if (!File.Exists(button.Path)) {
                        MessageBox.Show("File target could not be found. Please check target.");
                        return;
                    }
                    FileInfo targetFile = new FileInfo(button.Path);
                    string targetExt = targetFile.Extension;
                    string exePath = WinAPI.AssocQueryString(WinAPI.AssocStr.DDEApplication, targetExt) ?? "";
                    if (string.IsNullOrEmpty(WinAPI.AssocQueryString(WinAPI.AssocStr.DDEApplication, targetExt)) &&
                        string.IsNullOrEmpty(WinAPI.AssocQueryString(WinAPI.AssocStr.AppID, targetExt))) {
                        MessageBox.Show("Could not find default launcher for target file.");
                        return;
                    }
                    process = new Process {
                        StartInfo = {
                                FileName = button.Path,
                                Arguments = button.Arguments,
                                UseShellExecute = target.Extension != ".exe" || button.AdminOnly,
                                Verb = button.AdminOnly ? "runas" : ""
                            }
                    };
                    break;
                case LauncherButton.RefType.Webpage:
                    if (LauncherButton.BrowserPaths.Values.Contains(button.Path)) {
                        filename = button.Path;
                        arguments = button.Arguments;
                    } else {
                        filename = LauncherButton.BrowserPaths[button.TargetBrowser];
                        arguments = button.Path;
                    }
                    button.TargetBrowser = LauncherButton.GetBrowser(button);
                    if (button.TargetBrowser == LauncherButton.Browser.Edge) {
                        process = new Process {
                            StartInfo = {
                                    FileName = $@"{filename}{arguments}"
                                }
                        };
                    } else {
                        process = new Process {
                            StartInfo = {
                                FileName = filename,
                                Arguments = arguments,
                                UseShellExecute = false,
                                Verb = ""
                            }
                        };
                    }
                    break;
                case LauncherButton.RefType.Powershell:
                    target = new FileInfo(button.Path);
                    if (target.Extension == ".ps1") {
                        arguments = $@"-File ""{button.Path}"" -ExecutionPolicy Bypass";
                        filename = "powershell.exe";
                    } else {
                        filename = button.Path;
                        arguments = button.Arguments;
                    }
                    process = new Process {
                        StartInfo = {
                                FileName = filename,
                                Arguments = arguments,
                                UseShellExecute = button.AdminOnly,
                                Verb = button.AdminOnly ? "runas" : ""
                            }
                    };
                    break;
                case LauncherButton.RefType.Program:
                default:
                    process = new Process {
                        StartInfo = {
                                FileName = button.Path,
                                Arguments = button.Arguments,
                                UseShellExecute = target.Extension != ".exe" || button.AdminOnly,
                                Verb = button.AdminOnly ? "runas" : ""
                            }
                    };
                    break;

            }

            try {
                process.Start();
            } catch (System.ComponentModel.Win32Exception) {
                Debug.WriteLine("UAC Cancelled");
            }

            EnableWow64FSRedirection(true);
        }
        private void ButtonMouseDown(object s, System.Windows.Forms.MouseEventArgs e) {
            _dragSource = (LauncherButton)s;
            Debug.WriteLine("MouseDown event " + _dragSource.Text);
            _dragSource.DoDragDrop(_dragSource, DragDropEffects.Move);
        }
        private void ButtonMouseUp(object s, System.Windows.Forms.MouseEventArgs e) {
            LauncherButton button = (LauncherButton)s;
            Debug.WriteLine("MouseUp event " + button.Text);
            _dragSource = null;
        }
        private void ButtonDragEnter(object s, DragEventArgs e) {
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
        }
        private void ButtonDragDrop(object s, DragEventArgs e) {
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
        }
        public MainForm(string version) {
            _buttons = new ButtonCollection() {
                ButtonBuffer = ButtonBuffer,
                ButtonHeight = ButtonHeight,
                ButtonWidth = ButtonWidth,
                GridBufferHeight = MenuHeight + EditBuffer,
                GridBufferWidth = FormStaticBufferWidth
            };
            _click = ButtonClick;
            _mouseDown = ButtonMouseDown;
            _mouseUp = ButtonMouseUp;
            _dragEnter = ButtonDragEnter;
            _dragDrop = ButtonDragDrop;
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString;
            try {
                jsonString = File.ReadAllText($@"{path}\launcher.json");
            } catch (FileNotFoundException) {
                jsonString = $"[\r\n]";
                File.WriteAllText($@"{path}\launcher.json", jsonString);
            }
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
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);
            foreach (LauncherButton button in _buttons) {
                if (button.HasHotKeySet) {
                    hook.RegisterHotKey(button.KeyModifiers, button.Keys);
                }
            }
            _version = version;
            Text = string.Format(Resources.DefaultMainTitle, _version);
        }
        private void Hook_KeyPressed(object sender, KeyPressedEventArgs e) {
            foreach (LauncherButton button in _buttons) {
                if (button.HasHotKeySet) {
                    if (e.Modifier == button.KeyModifiers && e.Key == button.Keys)
                        ButtonClick(button, new EventArgs());
                }
            }
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
            Random random = new Random();
            int roll = random.Next(20) + 1;
            Text += $" - {roll}";
            if (roll == 20) {
                Dread dread = new Dread();
                dread.ShowDialog();
            }
            foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                if (!button.HasHotKeySet)
                    continue;
                try {
                    button.HotKeyId = hook.RegisterHotKey(button.KeyModifiers, button.Keys);
                } catch (Exception) { }
            }
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
            string jsonString = JsonConvert.SerializeObject(
                Controls
                    .OfType<LauncherButton>()
                    .Where(lb => lb.Caption != "" && lb.Path != "")
                    .OrderBy(lb => lb.GridLocation.Y)
                    .ThenBy(lb => lb.GridLocation.X), 
                Formatting.Indented);
            File.WriteAllText($@"{path}\launcher.json", jsonString);
            hook.Dispose();
        }
        private void EditToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            button.DetermineType();
            if (button.ReferenceType == LauncherButton.RefType.Webpage && button.TargetBrowser == LauncherButton.Browser.None)
                button.TargetBrowser = LauncherButton.GetBrowser(button);
            if (button.HotKeyId != -1)
                hook.UnregisterHotKey(button.HotKeyId);
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
                button.TargetBrowser = createOrEditButton.TargetBrowser;
                if (createOrEditButton.KeyModifiers != System.Windows.Input.ModifierKeys.None && createOrEditButton.HotkeyBase != System.Windows.Input.Key.None) {
                    button.HasHotKeySet = true;
                    button.KeyModifiers = createOrEditButton.KeyModifiers;
                    button.KeyTarget = createOrEditButton.HotkeyBase;
                    //button.HotKey = new HotKey(button.KeyModifiers, button.KeyTarget, Handle, (hotkey) => {
                    //    ButtonClick(button, new EventArgs());
                    //});
                    //_activeHotkeys.Add(button.HotKey);
                    button.HotKeyId = hook.RegisterHotKey(button.KeyModifiers, button.Keys);
                } else {
                    button.HasHotKeySet = false;
                    button.KeyModifiers = System.Windows.Input.ModifierKeys.None;
                    button.KeyTarget = System.Windows.Input.Key.None;
                }
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
                if (createOrEditButton.KeyModifiers != System.Windows.Input.ModifierKeys.None) {
                    button.HasHotKeySet = true;
                    button.KeyModifiers = createOrEditButton.KeyModifiers;
                    button.KeyTarget = createOrEditButton.HotkeyBase;
                    button.HotKeyId = hook.RegisterHotKey(button.KeyModifiers, button.Keys);
                } else {
                    button.HasHotKeySet = false;
                    button.KeyModifiers = System.Windows.Input.ModifierKeys.None;
                    button.KeyTarget = System.Windows.Input.Key.None;
                }
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
            _buttons.AddRange(
                JsonConvert.DeserializeObject<HashSet<LauncherButton>>(
                    JsonConvert.SerializeObject(
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
        private void RightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!(RightClickMenu.SourceControl is LauncherButton button))
                return;
            switch (button.ReferenceType) {
                case LauncherButton.RefType.Powershell:
                    runAsToolStripMenuItem.Enabled = true;
                    openTargetFolderToolStripMenuItem.Enabled = true;
                    copyArgumentsToolStripMenuItem.Enabled = true;
                    copyFullInvocationToolStripMenuItem.Enabled = true;
                    break;
                case LauncherButton.RefType.Program:
                    runAsToolStripMenuItem.Enabled = true;
                    openTargetFolderToolStripMenuItem.Enabled = true;
                    copyArgumentsToolStripMenuItem.Enabled = true;
                    copyFullInvocationToolStripMenuItem.Enabled = true;
                    break;
                case LauncherButton.RefType.Folder:
                    runAsToolStripMenuItem.Enabled = false;
                    openTargetFolderToolStripMenuItem.Enabled = true;
                    copyArgumentsToolStripMenuItem.Enabled = false;
                    copyFullInvocationToolStripMenuItem.Enabled = false;
                    break;
                case LauncherButton.RefType.Webpage:
                    runAsToolStripMenuItem.Enabled = false;
                    openTargetFolderToolStripMenuItem.Enabled = false;
                    copyArgumentsToolStripMenuItem.Enabled = false;
                    copyFullInvocationToolStripMenuItem.Enabled = false;

                    break;
                case LauncherButton.RefType.File:
                    runAsToolStripMenuItem.Enabled = false;
                    openTargetFolderToolStripMenuItem.Enabled = true;
                    copyArgumentsToolStripMenuItem.Enabled = false;
                    copyFullInvocationToolStripMenuItem.Enabled = false;
                    break;
                case LauncherButton.RefType.Undetermined:
                default:
                    runAsToolStripMenuItem.Enabled = true;
                    openTargetFolderToolStripMenuItem.Enabled = false;
                    break;
            }
        }
        private void copyTargetToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            Clipboard.Clear();
            Clipboard.SetText(button.Path, TextDataFormat.Text);
        }
        private void copyArgumentsToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            Clipboard.Clear();
            Clipboard.SetText(button.Arguments, TextDataFormat.Text);
        }
        private void copyFullInvocationToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            Clipboard.Clear();
            Clipboard.SetText(button.Path + " " + button.Arguments, TextDataFormat.Text);
        }
        private void openTargetFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            if (!File.Exists(button.Path))
                return;
            string targetDir;
            if (button.ReferenceType == LauncherButton.RefType.Folder)
                targetDir = button.Path;
            else
                targetDir = Path.GetDirectoryName(button.Path);
            Process process = new Process {
                StartInfo = {
                    FileName = NormalizePath(targetDir),
                    UseShellExecute = true,
                    Verb = "open"
                }
            };
            process.Start();
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
        private void CopyJSONToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            string jsonString = JsonConvert.SerializeObject(button, Formatting.Indented);
            Clipboard.Clear();
            Clipboard.SetText(jsonString, TextDataFormat.Text);
#if DEBUG
            MessageBox.Show(jsonString);
#endif
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
    }
}
