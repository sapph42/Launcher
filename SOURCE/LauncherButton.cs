using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Launcher {
    [JsonObject(MemberSerialization.OptIn)]
    public partial class LauncherButton : Button {
        private readonly Color _psBlue = Color.FromArgb(68, 119, 209);
        private readonly Color _psiseBlue = Color.FromArgb(94, 190, 221);
        private readonly Color _vsPurple = Color.FromArgb(189, 139, 243);
        private readonly Color _nppGreen = Color.FromArgb(147, 213, 67);
        private readonly Color _mmcRed = Color.FromArgb(196, 44, 42);
        private readonly Color _defaultBack;
        private readonly Color _defaultFore;

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
            if (Program.UsingDarkMode) {
                _defaultBack = SystemColors.ControlDarkDark;
                _defaultFore = Color.GhostWhite;
                FlatStyle = FlatStyle.Flat;
            }
            else {
                _defaultBack = SystemColors.Control;
                _defaultFore = SystemColors.ControlText;
                FlatStyle = FlatStyle.Standard;
            }
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
            if (Regex.IsMatch(Path, @"powershell\.exe") 
                || Regex.IsMatch(Path, @"ps1$")
               ) {
                BackColor = _psBlue;
                ForeColor = Color.GhostWhite;
            } else if (Regex.IsMatch(Path, @"mmc\.exe")
                || Regex.IsMatch(Path, @"msc$")
            ) {
                BackColor = _mmcRed;
                ForeColor = Color.GhostWhite;
            } else if (!string.IsNullOrEmpty(Path)) {
                FileInfo target = new FileInfo(Path);
                switch (target.Name) {
                    case "powershell_ise.exe":
                    case "PowerShell_ISE.exe":
                        BackColor = _psiseBlue;
                        ForeColor = SystemColors.ControlText;
                        break;
                    case "devenv.exe":
                        BackColor = _vsPurple;
                        ForeColor = SystemColors.ControlText;
                        break;
                    case "cmd.exe":
                        if (string.IsNullOrEmpty(Arguments)) {
                            BackColor = Color.Black;
                            ForeColor = Color.White;
                        }
                        else {
                            BackColor = _defaultBack;
                            ForeColor = _defaultFore;
                        }
                        break;
                    case "notepad++.exe":
                        BackColor = _nppGreen;
                        ForeColor = SystemColors.ControlText;
                        break;
                    default:
                        BackColor = _defaultBack;
                        ForeColor = _defaultFore;
                        break;
                }
            }
            else {
                BackColor = _defaultBack;
                ForeColor = _defaultFore;
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
