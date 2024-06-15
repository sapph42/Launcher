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
        private readonly Color _explorerYellow = Color.FromArgb(255, 214, 98);
        private readonly Color _ssmsOrange = Color.FromArgb(247, 179, 0);
        private readonly Color _bomgarOrange = Color.FromArgb(255, 85, 0);
        private readonly Color _edgeGreen = Color.FromArgb(69, 210, 154);
        private readonly Color _firefoxOrange = Color.FromArgb(255, 146, 29);
        private readonly Color _defaultFore;
        private readonly Color _lightFore = Color.GhostWhite;
        private readonly Color _darkFore = SystemColors.ControlText;

        public enum RefType {
            Undetermined,
            Program,
            Folder,
            Webpage,
            Powershell
        }

        public Color DefaultBack { get; }
        [JsonProperty]
        public string Caption {
            get => Text;
            set => Text = value;
        }
        [JsonProperty] public string Path { get; set; }
        [JsonProperty] public string Arguments { get; set; }
        [JsonProperty] public Point GridLocation { get; set; }
        [JsonProperty] public bool AdminOnly { get; set; } = false;
        [JsonProperty] public Color Background { get; set; } = SystemColors.Control;
        [JsonProperty] public RefType ReferenceType { get; set; }
        
        public LauncherButton() : this("", "", new Point()) { }

        [JsonConstructor]
        public LauncherButton(
            string caption, 
            string path, 
            Point grid, 
            string arguments = "", 
            bool adminOnly = false, 
            Color? background = null,
            RefType refType = RefType.Undetermined
        ) {
            Caption = caption;
            Path = path;
            Arguments = arguments;
            GridLocation = grid;
            AdminOnly = adminOnly;
            if (refType == RefType.Undetermined)
                DetermineType();
            InitializeComponent();
            if (Program.UsingDarkMode) {
                DefaultBack = SystemColors.ControlDarkDark;
                _defaultFore = _lightFore;
                FlatStyle = FlatStyle.Flat;
            }
            else {
                DefaultBack = SystemColors.Control;
                _defaultFore = _darkFore;
                FlatStyle = FlatStyle.Standard;
            }
            Background = background ?? DefaultBack;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
        }
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            SetForeColor();
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
        private void SetForeColor() {
            ForeColor = BackColor.GetContrastRatio(_lightFore) > BackColor.GetContrastRatio(_darkFore) ? _lightFore : _darkFore;
        }
        public void ColorCheck() {
            if (Background != DefaultBack) {
                BackColor = Background;
            } else if (string.IsNullOrEmpty(Path)) {
                BackColor = DefaultBack;
            } else if (Regex.IsMatch(Path, @"powershell\.exe")
                || Regex.IsMatch(Path, @"ps1$")
               ) {
                BackColor = _psBlue;
            } else if (Regex.IsMatch(Path, @"mmc\.exe")
                       || Regex.IsMatch(Path, @"msc$")
                      ) {
                BackColor = _mmcRed;
            } else if (!string.IsNullOrEmpty(Path)) {
                FileInfo target = new FileInfo(Path);
                switch (target.Name.ToLowerInvariant()) {
                    case "powershell_ise.exe":
                        BackColor = _psiseBlue;
                        break;
                    case "devenv.exe":
                        BackColor = _vsPurple;
                        break;
                    case "cmd.exe":
                        BackColor = string.IsNullOrEmpty(Arguments) ? Color.Black : DefaultBack;
                        break;
                    case "notepad++.exe":
                        BackColor = _nppGreen;
                        break;
                    case "explorer.exe":
                        BackColor = _explorerYellow;
                        break;
                    case "ssms.exe":
                        BackColor = _ssmsOrange;
                        break;
                    case "bomgar-rep.exe":
                        BackColor = _bomgarOrange;
                        break;
                    case "firefox.exe":
                        BackColor = _firefoxOrange;
                        break;
                    case "microsoft-edge:":
                    case "chrome.exe":
                        BackColor = _edgeGreen;
                        break;
                    default:
                        BackColor = DefaultBack;
                        break;
                }
            } else {
                BackColor = DefaultBack;
            }
            Background = BackColor;
            SetForeColor();
        }
        public static RefType DetermineType(LauncherButton button) {
            try {
                try {
                    if (string.IsNullOrEmpty(button.Path)) {
                        button.Path = button.Arguments;
                        button.Arguments = null;
                    }
                    FileAttributes fileAttributes = File.GetAttributes(button.Path);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                        return RefType.Folder;
                    else {
                        FileInfo fileInfo = new FileInfo(button.Path);
                        if (fileInfo.Extension == ".exe" && !Regex.IsMatch(button.Arguments, @"(?<startargs>.*)(?:-File\s)""?(?<path>[^""]*)""?\s(?<endargs>.*)"))
                            return RefType.Program;
                        else if (
                            fileInfo.Extension == ".ps1" || (
                                fileInfo.Name == "powershell.exe" && 
                                Regex.IsMatch(button.Arguments, @"(?<startargs>.*)(?:-File\s)""?(?<path>[^""]*)""?\s(?<endargs>.*)")
                            )
                        )
                            return RefType.Powershell;
                        return RefType.Undetermined;
                    }
                } catch { //NOT A FILE OR DIRECTORY
                    try {
                        if (Uri.IsWellFormedUriString(button.Path, UriKind.Absolute)) {
                            Uri uri = new Uri(button.Path);
                            if (uri.HostNameType == UriHostNameType.Dns && !uri.IsUnc)
                                return RefType.Webpage;
                        }
                        return RefType.Undetermined;
                    } catch {
                        return RefType.Undetermined;
                    }
                }
            } catch {
                return RefType.Undetermined;
            }
        }
        private void DetermineType() {
            try {
                try {
                    FileAttributes fileAttributes = File.GetAttributes(Path);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                        ReferenceType = RefType.Folder;
                    else {
                        FileInfo fileInfo = new FileInfo(Path);
                        if (fileInfo.Extension == ".exe")
                            ReferenceType = RefType.Program;
                        else if (fileInfo.Extension == ".ps1")
                            ReferenceType = RefType.Powershell;
                        else
                            ReferenceType = RefType.Undetermined;
                    }
                } catch { //NOT A FILE OR DIRECTORY
                    try {
                        if (Uri.IsWellFormedUriString(Path, UriKind.Absolute)) {
                            Uri uri = new Uri(Path);
                            if (uri.HostNameType == UriHostNameType.Dns && !uri.IsUnc)
                                ReferenceType = RefType.Webpage;
                        } else
                            ReferenceType = RefType.Undetermined;
                    } catch {
                        ReferenceType = RefType.Undetermined;
                    }
                }
            } catch {
                ReferenceType = RefType.Undetermined;
            }
        }
        public void Assimilate(LauncherButton drone) {
            Caption = drone.Caption;
            Path = drone.Path;
            Arguments = drone.Arguments;
            GridLocation = drone.GridLocation;
            Location = drone.Location;
            AdminOnly = drone.AdminOnly;
        }
    }
}
