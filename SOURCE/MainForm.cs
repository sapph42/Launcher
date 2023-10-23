using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
        private bool RearrangeMode = false;

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
                Debug.WriteLine("Click event " + button.Text);
                Process process = new Process {
                    StartInfo = {
                        FileName = button.Tag.ToString(),
                        UseShellExecute = button.Parent.Name == "adminPage" ? true : false,
                        Verb = button.Parent.Name == "adminPage" ? "runas" : ""
                    }
                };
                process.Start();
                EnableWow64FSRedirection(true);
            };
            _mouseDown = delegate(object s, MouseEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("MouseDown event " + button.Text);
                button.DoDragDrop(button.Text, DragDropEffects.Move);
            };
            _mouseUp = delegate(object s, MouseEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("MouseUp event " + button.Text);
            };
            _dragEnter = delegate(object s, DragEventArgs e) {
                Button button = (Button)s;
                Debug.WriteLine("DragEnter event " + button.Text);
                if (
                    e.Data.GetDataPresent(DataFormats.Text) 
                    && s != null
                    && _buttons.Any(b => b.Caption == e.Data.GetData(DataFormats.Text).ToString())
                )
                    e.Effect = DragDropEffects.Move;
                else
                    e.Effect = DragDropEffects.None;
            };
            _dragDrop = delegate(object s, DragEventArgs e) {
                string otherCaption = e.Data.GetData(DataFormats.Text).ToString();
                Button button = (Button)s;
                Button target = button.Parent.Controls.OfType<Button>().First(b => b.Text == otherCaption);
                Debug.WriteLine("DragDrop event " + button.Text);
                ButtonInfo button1 = _buttons.First(b => b.Caption == button.Text);
                ButtonInfo button2 = _buttons.First(b => b.Caption == otherCaption);
                _buttons.Rearrange(button1, button2);
                string textHold = button.Text;
                string pathHold = button.Tag.ToString();
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

        private void DrawButtons() {
            tabControl.SuspendLayout();
            standardPage.SuspendLayout();
            adminPage.SuspendLayout();
            SuspendLayout();
            //foreach (Control control in standardPage.Controls) {
            //    if (control is Button)
            //        standardPage.Controls.Remove(control);
            //}
            //foreach (Control control in adminPage.Controls) {
            //    if (control is Button)
            //        standardPage.Controls.Remove(control);
            //}
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
            foreach (ButtonInfo button in _buttons) {
                button.StandardControl.Click += _click;
                button.AdminControl.Click += _click;
                //button.StandardControl.MouseDown += _mouseDown;
                //button.AdminControl.MouseDown += _mouseDown;
                button.StandardControl.MouseUp += _mouseUp;
                button.AdminControl.MouseUp += _mouseUp;
                button.StandardControl.DragEnter += _dragEnter;
                button.AdminControl.DragEnter += _dragEnter;
                button.StandardControl.DragDrop += _dragDrop;
                button.AdminControl.DragDrop += _dragDrop;

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
            if (RearrangeMode) {
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

            RearrangeMode = !RearrangeMode;
        }
    }
}
