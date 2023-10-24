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
        private const int TabStaticBufferWidth = 8;
        private const int TabStaticBufferHeight = 26;
        private const int TabControlBufferWidth = 20;
        private const int TabControlBufferHeight = 12;
        private ButtonCollection _buttons;
        private readonly EventHandler _click;
        private readonly MouseEventHandler _mouseDown;
        private readonly MouseEventHandler _mouseUp;
        private readonly DragEventHandler _dragEnter;
        private readonly DragEventHandler _dragDrop;
        private bool _rearrangeMode = false;

        [System.Runtime.InteropServices.DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        public static extern bool EnableWow64FSRedirection(bool enable);

        public MainForm() {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString = File.ReadAllText($@"{path}\launcher.json");
            _buttons = new ButtonCollection() {
                Buffer = ButtonBuffer,
                Height = ButtonHeight,
                Width = ButtonWidth
            };
            _click = delegate(object s, EventArgs e) {
                EnableWow64FSRedirection(false);
                Button button = (Button)s;
                ButtonInfo parent = (ButtonInfo)button.Tag;
                if (string.IsNullOrEmpty(button.Text))
                    return;
                Debug.WriteLine("Click event " + button.Text);
                if (string.IsNullOrEmpty(parent.Path))
                    return;
                FileInfo target = new FileInfo(parent.Path);
                if (target.Extension == ".ps1") {
                    Process process = new Process {
                        StartInfo = {
                            FileName = "powershell.exe",
                            Arguments = $@"-File {button.Tag} -ExecutionPolicy Bypass",
                            UseShellExecute = button.Parent.Name == "adminPage",
                            Verb = button.Parent.Name == "adminPage" ? "runas" : ""
                        }
                    };
                } else {
                    Process process = new Process {
                        StartInfo = {
                            FileName = parent.Path,
                            UseShellExecute = button.Parent.Name == "adminPage",
                            Verb = button.Parent.Name == "adminPage" ? "runas" : ""
                        }
                    };
                    try {
                        process.Start();
                    }
                    catch (System.ComponentModel.Win32Exception) {
                        Debug.WriteLine("UAC Cancelled");
                    }
                }
                    
                EnableWow64FSRedirection(true);
            };
            _mouseDown = delegate(object s, MouseEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("MouseDown event " + button.Text);
                button.DoDragDrop(button.Tag, DragDropEffects.Move);
            };
            _mouseUp = delegate(object s, MouseEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("MouseUp event " + button.Text);
            };
            _dragEnter = delegate(object s, DragEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("DragEnter event " + button.Text);
                if (
                    e.Data.GetDataPresent(DataFormats.Serializable) 
                    && s != null
                    && _buttons.Any(b => b.Equals(e.Data.GetData(DataFormats.Serializable)))
                )
                    e.Effect = DragDropEffects.Move;
                else
                    e.Effect = DragDropEffects.None;
            };
            _dragDrop = delegate(object s, DragEventArgs e) {
                Button button = (Button)s;
                ButtonInfo button1 = (ButtonInfo)button.Tag;
                ButtonInfo button2 = (ButtonInfo)e.Data.GetData(DataFormats.Serializable);
                Button target = button.Name.Substring(0, 3) == "std"
                    ? button2.StandardControl
                    : button2.AdminControl;
                Debug.WriteLine("DragDrop event " + button.Text);

                _buttons.Rearrange(button1, button2);
                string textHold = button.Text;
                ButtonInfo pathHold = (ButtonInfo)button.Tag;
                button.Text = target.Text;
                button.Tag = target.Tag;
                target.Text = textHold;
                target.Tag = pathHold;
            };

            _buttons.AddRange(JsonConvert.DeserializeObject<List<ButtonInfo>>(jsonString));
            _buttons.Validate();
            InitializeComponent();
            DrawButtons();
        }

        private void DrawButtons(bool redraw = false) {
            tabControl.SuspendLayout();
            standardPage.SuspendLayout();
            adminPage.SuspendLayout();
            SuspendLayout();
            if (redraw) {
                foreach (Control control in standardPage.Controls) {
                    if (control is Button)
                        standardPage.Controls.Remove(control);
                }

                foreach (Control control in adminPage.Controls) {
                    if (control is Button)
                        adminPage.Controls.Remove(control);
                }
            }
            Size pageControlSize = new Size() {
                Width = ButtonBuffer + ((ButtonBuffer + ButtonWidth) * _buttons.X),
                Height = ButtonBuffer + ((ButtonBuffer + ButtonHeight) * _buttons.Y)
            };
            tabControl.Size = new Size() {
                Width = TabStaticBufferWidth + pageControlSize.Width,
                Height = TabStaticBufferHeight + pageControlSize.Height
            };
            tabControl.Size = tabControl.Size;
            adminPage.Size = pageControlSize;
            standardPage.Size = pageControlSize;
            int titleBarHeight = Height - ClientRectangle.Height;
            Size = new Size() {
                Width = TabControlBufferWidth + tabControl.Size.Width + TabControlBufferWidth,
                Height = TabControlBufferHeight + tabControl.Size.Height + TabControlBufferHeight + titleBarHeight
            };
            SwapButton.Location = new Point(Size.Width - 54, SwapButton.Location.Y);
            AddButton.Location = new Point(SwapButton.Location.X - 30, AddButton.Location.Y);
            foreach (ButtonInfo button in _buttons) {
                button.StandardControl.Click += _click;
                button.AdminControl.Click += _click;
                button.StandardControl.MouseUp += _mouseUp;
                button.AdminControl.MouseUp += _mouseUp;
                button.StandardControl.DragEnter += _dragEnter;
                button.AdminControl.DragEnter += _dragEnter;
                button.StandardControl.DragDrop += _dragDrop;
                button.AdminControl.DragDrop += _dragDrop;
                button.StandardControl.ContextMenuStrip = ContextMenuStrip;
                button.AdminControl.ContextMenuStrip = ContextMenuStrip;

                standardPage.Controls.Add(button.StandardControl);
                adminPage.Controls.Add(button.AdminControl);
            }
            tabControl.ResumeLayout(false);
            standardPage.ResumeLayout(false);
            adminPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString = JsonConvert.SerializeObject(_buttons, Formatting.Indented);
            File.WriteAllText($@"{path}\launcher.json", jsonString);
        }

        private void SwapButton_Click(object sender, EventArgs e) {
            if (_rearrangeMode) {
                SwapButton.BackgroundImage = Properties.Resources.SwapIcon;
                foreach (Button button in standardPage.Controls.OfType<Button>()) {
                    button.MouseDown -= _mouseDown;
                }
                foreach (Button button in adminPage.Controls.OfType<Button>()) {
                    button.MouseDown -= _mouseDown;
                }
            }
            else {
                SwapButton.BackgroundImage = Properties.Resources.Confirm;
                foreach (Button button in standardPage.Controls.OfType<Button>()) {
                    button.MouseDown += _mouseDown;
                }
                foreach (Button button in adminPage.Controls.OfType<Button>()) {
                    button.MouseDown += _mouseDown;
                }
            }

            _rearrangeMode = !_rearrangeMode;
        }

        private void AddButton_Click(object sender, EventArgs e) {
            CreateOrEditButton createOrEditButton = new CreateOrEditButton();
            createOrEditButton.ShowDialog();
            if (createOrEditButton.DialogResult == DialogResult.Cancel)
                return;
            ButtonInfo newButton = new ButtonInfo(createOrEditButton.Caption, createOrEditButton.Path);
            _buttons.Add(newButton);
            _buttons.Validate();
            DrawButtons(true);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item is null)
                return;
            ContextMenuStrip strip = item.GetCurrentParent() as ContextMenuStrip;
            Button button = strip.SourceControl as Button;
            if (button is null)
                return;
            ButtonInfo buttonInfo = (ButtonInfo)button.Tag;
            CreateOrEditButton createOrEditButton = new CreateOrEditButton(buttonInfo.Path, buttonInfo.Caption);
            createOrEditButton.ShowDialog();
            if (createOrEditButton.DialogResult == DialogResult.Cancel)
                return;
            button.Text = createOrEditButton.Caption;
            _buttons.EditButton(buttonInfo, createOrEditButton.Caption, createOrEditButton.Path);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item is null)
                return;
            ContextMenuStrip strip = item.GetCurrentParent() as ContextMenuStrip;
            Button button = strip.SourceControl as Button;
            if (button is null)
                return;
            ButtonInfo buttonInfo = (ButtonInfo)button.Tag;
            _buttons.ClearButton(buttonInfo);
            button.Text = "";
        }
    }
}
