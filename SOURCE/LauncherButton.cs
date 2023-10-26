using System;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Launcher {
    [JsonObject(MemberSerialization.OptIn)]
    public partial class LauncherButton : Button {
        private readonly Color _psBlue = Color.FromArgb(68, 119, 209);

        [JsonProperty]
        public string Caption {
            get => Text;
            set => Text = value;
        }
        [JsonProperty] public string Path { get; set; }
        [JsonProperty] public string Arguments { get; set; }
        [JsonProperty] public Point GridLocation { get; set; }
        
        public LauncherButton() : this("", "", new Point(), "") { }

        [JsonConstructor]
        public LauncherButton(string caption, string path, Point grid, string arguments = "") {
            Caption = caption;
            Path = path;
            Arguments = arguments;
            GridLocation = grid;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
        }

        public object this[string propertyName] {
            get {
                Type myType = typeof(LauncherButton);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                return myPropertyInfo?.GetValue(this, null);
            }
            set {
                Type myType = typeof(LauncherButton);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                myPropertyInfo?.SetValue(this, value, null);
            }
        }

        public void ColorCheck() {
            if (
                (
                    Regex.IsMatch(Path, @"powershell\.exe") 
                    && Regex.IsMatch(Arguments, "ps1")
                ) 
                || Regex.IsMatch(Path, @"ps1$")
               ) {
                BackColor = _psBlue;
                ForeColor = Color.GhostWhite;
            } else {
                BackColor = SystemColors.Control;
                ForeColor = SystemColors.ControlText;
            }
        }

        public void Assimilate(LauncherButton drone) {
            Caption = drone.Caption;
            Path = drone.Path;
            Arguments = drone.Arguments;
            GridLocation = drone.GridLocation;
            Location = drone.Location;
        }
    }
}
