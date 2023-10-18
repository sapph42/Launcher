using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Launcher {
    public partial class MainForm : Form {
        
        private struct ButtonInfo {
            public string Caption; //Caption is basically Button.Text
            public string Path; //Path is what will be passed to Process.Start
            public Point GridLocation;  //Where on the virtual grid the button will be placed
            public Button StandardControl; //The actual instantiation that will be placed on standardPage
            public Button AdminControl; //The actual instantiation that will be placed on adminPage
        }

        private const int ButtonWidth = 94;
        private const int ButtonHeight = 52;
        private const int ButtonBuffer = 6; //Distance between buttons and surrounding elements
        private const int TabStaticBufferWidth = 8;  //The static differences between the size of a TabPage and a TabControl
        private const int TabStaticBufferHeight = 26;
        private const int TabControlBufferWidth = 20; //The static differences between the size of the TabControl and the Form
        private const int TabControlBufferHeight = 12;

        public MainForm() {
            //Read the json file in and deserialize to our ButtonInfo list
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var jsonString = File.ReadAllText($@"{path}\launcher.json");
            var buttons = JsonConvert.DeserializeObject<List<ButtonInfo>>(jsonString);

            var buttonGridWidth = 1;
            var buttonGridHeight = 1;

            //Calculate the size of the virtual grid that buttons will be placed on
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
                    var ratio = (float)buttons.Count / buttonGridWidth;
                    var prelimHeight = (int)Math.Round(ratio);
                    var prelimArea = prelimHeight * buttonGridWidth;
                    var offset = prelimArea < buttons.Count ? buttons.Count - prelimArea : 0;
                    buttonGridHeight = prelimHeight + offset;
                    break;
            }

            //GridLocation Data may or may not be valid in JSON, so we are going to recalc it
            //We iterate through the List, while also stepping through the available grid positions
            var x = 1;
            var y = 1;
            for (var i = 0; i < buttons.Count; i++) {
                if (y > buttonGridHeight) {
                    y = 1;
                    x++;
                }
                buttons[i] = new ButtonInfo() {
                    Caption = buttons[i].Caption,
                    Path = buttons[i].Path,
                    GridLocation = new Point(x, y)
                };
                y++;
            }

            InitializeComponent();

            tabControl.SuspendLayout();
            standardPage.SuspendLayout();
            adminPage.SuspendLayout();
            SuspendLayout();

            var pageControlSize = new Size() {
                Width = ButtonBuffer + ((ButtonBuffer + ButtonWidth) * buttonGridWidth),
                Height = ButtonBuffer + ((ButtonBuffer + ButtonHeight) * buttonGridHeight)
            };
            adminPage.Size = pageControlSize;
            standardPage.Size = pageControlSize;
            
            tabControl.Size = new Size() {
                Width = TabStaticBufferWidth + pageControlSize.Width,
                Height = TabStaticBufferHeight + pageControlSize.Height
            };
            tabControl.Size = tabControl.Size;
            
            var titleBarHeight = Height - ClientRectangle.Height; //We have to account for the size of the title bar
            Size = new Size() {
                Width = TabControlBufferWidth + tabControl.Size.Width + TabControlBufferWidth,
                Height = TabControlBufferHeight + tabControl.Size.Height + TabControlBufferHeight + titleBarHeight
            };

            //Now that the container elements are size properly, lets build and place the buttons
            for (var i = 0; i < buttons.Count; i++) {
                buttons[i] = new ButtonInfo() {
                    Caption = buttons[i].Caption,
                    Path = buttons[i].Path,
                    GridLocation = buttons[i].GridLocation,
                    StandardControl = ButtonBuilder(buttons[i], i, "standard"),
                    AdminControl = ButtonBuilder(buttons[i], i, "admin")
                };
                standardPage.Controls.Add(buttons[i].StandardControl);
                adminPage.Controls.Add(buttons[i].AdminControl);
            }

            tabControl.ResumeLayout(false);
            standardPage.ResumeLayout(false);
            adminPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Button ButtonBuilder(ButtonInfo button, int index, string page) {
            var newButton = new Button();
            var buttonLocation = new Point(
                (ButtonBuffer * button.GridLocation.X) + (ButtonWidth * (button.GridLocation.X - 1)),
                (ButtonBuffer * button.GridLocation.Y) + (ButtonHeight * (button.GridLocation.Y - 1))
            );
            newButton.Location = buttonLocation;
            //The name isn't super relevant, so this is just a way to make sure buttons have unique names
            newButton.Name = $"{page}Button_{button.GridLocation.X}_{button.GridLocation.Y}";
            newButton.Size = new Size(ButtonWidth, ButtonHeight);
            newButton.TabIndex = index;
            //Tag is where we store the path, since the event handler won't have a way of connecting a control to the ButtonInfo obj
            newButton.Tag = button.Path;
            newButton.Text = button.Caption;
            newButton.UseVisualStyleBackColor = true;
            //All buttons call the same EventHandler, since they are dynamically generated
            newButton.Click += ButtonClick;
            return newButton;
        }

        private static void ButtonClick(object sender, EventArgs e) {
            var button = (Button)sender;
            var path = button.Tag.ToString();
            var process = new Process {
                StartInfo = {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = button.Parent.Name=="adminPage"?"runas":""
                }
            };
            process.Start();
        }
    }
}
