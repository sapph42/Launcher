using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Launcher {
    public partial class MainForm : Form {
        //private Dictionary<string, string> buttons = new Dictionary<string, string>();

        private struct ButtonInfo {
            public string caption;
            public string path;
            public Point gridLocation;
            public Button standardControl;
            public Button adminControl;
        }

        private const int ButtonWidth = 94;
        private const int ButtonHeight = 52;
        private const int ButtonBuffer = 6;
        private const int TabStaticBufferWidth = 8;
        private const int TabStaticBufferHeight = 26;
        private const int TabControlBufferWidth = 20;
        private const int TabControlBufferHeight = 12;

        private readonly List<ButtonInfo> buttons = new List<ButtonInfo>();
        public MainForm() {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jsonString = File.ReadAllText($@"{path}\launcher.json");
            buttons = JsonConvert.DeserializeObject<List<ButtonInfo>>(jsonString);
            int buttonGridWidth = 1;
            int buttonGridHeight = 1;
            switch (buttons.Count) {
                case 1:
                    buttonGridHeight = 1;
                    buttonGridWidth = 1;
                    break;
                case 2:
                    buttonGridHeight = 1;
                    buttonGridWidth = 2;
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    buttonGridWidth = (int)Math.Round(Math.Sqrt(buttons.Count));
                    float ratio = (float)buttons.Count / buttonGridWidth;
                    int prelimHeight = (int)Math.Round(ratio);
                    int prelimArea = prelimHeight * buttonGridWidth;
                    int offset = prelimArea < buttons.Count ? buttons.Count - prelimArea : 0;
                    buttonGridHeight = prelimHeight + offset;
                    break;
            }

            int x = 1;
            int y = 1;
            for (int i = 0; i < buttons.Count; i++) {
                if (y > buttonGridHeight) {
                    y = 1;
                    x++;
                }
                buttons[i] = new ButtonInfo() {
                    caption = buttons[i].caption,
                    path = buttons[i].path,
                    gridLocation = new Point(x, y)
                };
                y++;
            }

            InitializeComponent();

            tabControl.SuspendLayout();
            standardPage.SuspendLayout();
            adminPage.SuspendLayout();
            SuspendLayout();
            Size pageControlSize = new Size() {
                Width = ButtonBuffer + ((ButtonBuffer + ButtonWidth) * buttonGridWidth),
                Height = ButtonBuffer + ((ButtonBuffer + ButtonHeight) * buttonGridHeight)
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
            for (int i = 0; i < buttons.Count; i++) {
                buttons[i] = new ButtonInfo() {
                    caption = buttons[i].caption,
                    path = buttons[i].path,
                    gridLocation = buttons[i].gridLocation,
                    standardControl = ButtonBuilder(buttons[i], i, "standard"),
                    adminControl = ButtonBuilder(buttons[i], i, "admin")
                };
                standardPage.Controls.Add(buttons[i].standardControl);
                adminPage.Controls.Add(buttons[i].adminControl);
            }

            tabControl.ResumeLayout(false);
            standardPage.ResumeLayout(false);
            adminPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Button ButtonBuilder(ButtonInfo button, int index, string page) {
            Button newButton = new Button();
            Point buttonLocation = new Point(
                (ButtonBuffer * button.gridLocation.X) + (ButtonWidth * (button.gridLocation.X - 1)),
                (ButtonBuffer * button.gridLocation.Y) + (ButtonHeight * (button.gridLocation.Y - 1))
            );
            newButton.Location = buttonLocation;
            newButton.Name = $"{page}Button_{button.gridLocation.X}_{button.gridLocation.Y}";
            newButton.Size = new Size(ButtonWidth, ButtonHeight);
            newButton.TabIndex = index;
            newButton.Tag = button.path;
            newButton.Text = button.caption;
            newButton.UseVisualStyleBackColor = true;
            newButton.Click += new EventHandler(ButtonClick);
            return newButton;
        }

        private void ButtonClick(object sender, EventArgs e) {
            Button button = (Button)sender;
            string Path = button.Tag.ToString();
            Process process = new Process {
                StartInfo = {
                    FileName = Path,
                    UseShellExecute = true,
                    Verb = button.Parent.Name=="adminPage"?"runas":""
                }
            };
            process.Start();
        }
    }
}
