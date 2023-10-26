using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
        private string _version;

        [System.Runtime.InteropServices.DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        public static extern bool EnableWow64FSRedirection(bool enable);

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
                        UseShellExecute = button.Parent.Name == "adminPage",
                        Verb = button.Parent.Name == "adminPage" ? "runas" : ""
                    }
                };
         
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

                _dragSource.Caption = copyOfDestination.Caption;
                _dragSource.Path = copyOfDestination.Path;
                _dragSource.Arguments = copyOfDestination.Arguments;
                _dragSource = null;
                RefreshCollection();
            };

            _buttons.AddRange(JsonConvert.DeserializeObject<HashSet<LauncherButton>>(jsonString));
            if (_buttons.Select(b => b.GridLocation).Count() >
                _buttons.Select(b => b.GridLocation).Distinct().Count()) {
                MessageBox.Show(
                    "JSON entries are in an inconsistant state.  Grid locations will be reset, and size recalculated.");
                foreach (LauncherButton button in _buttons)
                    button.GridLocation = new Point(0, 0);
                _buttons.Validate(true);
            } else 
                _buttons.Validate();
            InitializeComponent();
            DrawButtons();
            _version = version;
            Text = $"Launcher - {_version}";
        }

        public sealed override string Text {
            get => base.Text;
            set => base.Text = value;
        }

        private void DrawButtons(bool redraw = false) {
            SuspendLayout();
            if (redraw) {
                while (Controls.OfType<LauncherButton>().Count() > 0) {
                    foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                        Controls.Remove(Controls.Find(button.Name, true).LastOrDefault());
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
            CreateOrEditButton createOrEditButton = new CreateOrEditButton(button.Path, button.Caption, button.Arguments);
            createOrEditButton.ShowDialog();
            if (createOrEditButton.DialogResult == DialogResult.Cancel)
                return;
            button.Caption = createOrEditButton.Caption;
            button.Path = createOrEditButton.Path;
            button.Arguments = createOrEditButton.Arguments;
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

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item?.GetCurrentParent() as ContextMenuStrip;
            if (!(strip?.SourceControl is LauncherButton button))
                return;
            button.Caption = "";
            button.Path = "";
            button.Arguments = "";
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
                Text = $"Launcher - {_version}";
                foreach (LauncherButton button in Controls.OfType<LauncherButton>()) {
                    button.MouseDown -= _mouseDown;
                }
            } else {
                Text = $"Launcher - {_version} - Rearrange Mode";
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
