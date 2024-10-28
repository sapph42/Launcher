using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Input;

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
        private readonly Color _lightFore = Color.GhostWhite;
        private readonly Color _darkFore = SystemColors.ControlText;

        public enum RefType {
            Undetermined,
            Program,
            Folder,
            Webpage,
            Powershell,
            File
        }

        public enum Browser {
            None,
            Edge,
            Firefox,
            Chrome
        }

        public static Dictionary<Browser, string> BrowserPaths = new Dictionary<Browser, string> {
            { Browser.None, null },
            { Browser.Edge, "microsoft-edge:" }, 
            { Browser.Firefox, @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe" },
            { Browser.Chrome, @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" }
        };

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
        [JsonProperty] public Browser TargetBrowser { get; set; }
        [JsonProperty] public bool HasHotKeySet { get; set; } = false;
        [JsonProperty] public System.Windows.Input.ModifierKeys KeyModifiers { get; set; } = System.Windows.Input.ModifierKeys.None;
        [JsonProperty] public Key KeyTarget { get; set; } = Key.None;
        public Keys Keys => (Keys)KeyInterop.VirtualKeyFromKey(KeyTarget);
        public int HotKeyId { get; set; } = -1;
        
        public LauncherButton() : this("", "", new Point()) { }

        [JsonConstructor]
        public LauncherButton(
            string caption, 
            string path, 
            Point grid, 
            string arguments = "", 
            bool adminOnly = false, 
            Color? background = null,
            RefType refType = RefType.Undetermined,
            Browser browserTarget = Browser.None
        ) {
            Caption = caption;
            Path = path;
            Arguments = arguments;
            GridLocation = grid;
            AdminOnly = adminOnly;
            ReferenceType = refType;
            if (ReferenceType == RefType.Webpage && TargetBrowser == Browser.None)
                TargetBrowser = Browser.Edge;
            else
                TargetBrowser = browserTarget;
            NormalizeFields();
            InitializeComponent();
            if (Program.UsingDarkMode) {
                DefaultBack = SystemColors.ControlDarkDark;
                FlatStyle = FlatStyle.Flat;
            }
            else {
                DefaultBack = SystemColors.Control;
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
            button.DetermineType();
            return button.ReferenceType;
        }
        public void DetermineType() {
            try {
                try {
                    FileAttributes fileAttributes = File.GetAttributes(Path);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                        ReferenceType = RefType.Folder;
                    else {
                        FileInfo fileInfo = new FileInfo(Path);
                        if (BrowserPaths.Values.Contains(fileInfo.FullName) || Path.StartsWith("http"))
                            ReferenceType = RefType.Webpage;
                        else if (fileInfo.Extension == ".exe") 
                            ReferenceType = RefType.Program;
                        else if (fileInfo.Extension == ".ps1")
                            ReferenceType = RefType.Powershell;
                        else
                            ReferenceType = RefType.File;
                    }
                } catch { //NOT A FILE OR DIRECTORY
                    if (Path == "powershell.exe" && !string.IsNullOrEmpty(Arguments)) {
                        ReferenceType = RefType.Powershell;
                        return;
                    } else if (Path == "microsoft-edge:") {
                        ReferenceType = RefType.Webpage;
                    }
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
        public static Browser GetBrowser(LauncherButton button) {
            if (button.TargetBrowser != Browser.None)
                return button.TargetBrowser;
            return button.Path switch {
                var PathVal when new Regex(@".*\\firefox\.exe").IsMatch(PathVal) => Browser.Firefox,
                var PathVal when new Regex(@".*\\chrome\.exe").IsMatch(PathVal) => Browser.Chrome,
                "microsoft-edge:" => Browser.Edge,
                _ => Browser.None,
            };
        }
        private void SetBrowser() {
            if (Uri.TryCreate(Path, UriKind.Absolute, out _)) {
                if (TargetBrowser == Browser.None)
                    TargetBrowser = Browser.Edge;
            } else {
                TargetBrowser = Path switch {
                    var PathVal when new Regex(@".*\\firefox\.exe").IsMatch(PathVal) => Browser.Firefox,
                    var PathVal when new Regex(@".*\\chrome\.exe").IsMatch(PathVal) => Browser.Chrome,
                    "microsoft-edge:" => Browser.Edge,
                    _ => Browser.None,
                };
            }
        }
        public static string[] ExtractPSPathFromArgument(string argument) {
            Regex QuotedArgumentParser = new Regex(@"(?<startargs>.*)(?:-File\s)""(?<path>[^""]*)""\s(?<endargs>.*)");
            Regex UnquotedArgumentParser = new Regex(@"(?<startargs>.*)(?:-File\s)(?<path>[^"" ]*)\s(?<endargs>.*)");
            MatchCollection matches;
            string[] arguments = new string[2];
            if (QuotedArgumentParser.IsMatch(argument)) {
                matches = QuotedArgumentParser.Matches(argument);
                var groups = matches[0].Groups;
                arguments[0] = groups["path"].Value;
                if (!string.IsNullOrEmpty(groups["startargs"].Value)
                    && !string.IsNullOrEmpty(groups["endargs"].Value)) {
                    arguments[1] = $"{groups["startargs"].Value} {groups["endargs"].Value}";
                } else
                    arguments[1] = groups["startargs"].Value + groups["endargs"].Value;
            } else if (UnquotedArgumentParser.IsMatch(argument)) {
                matches = UnquotedArgumentParser.Matches(argument);
                var groups = matches[0].Groups;
                arguments[0] = groups["path"].Value;
                if (!string.IsNullOrEmpty(groups["startargs"].Value)
                    && !string.IsNullOrEmpty(groups["endargs"].Value)) {
                    arguments[1] = $"{groups["startargs"].Value} {groups["endargs"].Value}";
                } else
                    arguments[1] = groups["startargs"].Value ?? groups["endargs"].Value;
            } else {
                arguments[0] = "";
                arguments[1] = argument;
            }
            return arguments;
        }
        public void NormalizeFields() {
            DetermineType();
            switch (ReferenceType) {
                case RefType.Program:
                    if (BrowserPaths.Values.Contains(Path) && !string.IsNullOrEmpty(Arguments)) {
                        ReferenceType = RefType.Webpage;
                        SetBrowser();
                        Path = Arguments;
                        Arguments = "";
                    }
                    if (Path == "powershell.exe")
                        Path = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
                    if (Path == "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe" && !string.IsNullOrEmpty(Arguments)) {
                        var parsed = ExtractPSPathFromArgument(Arguments);
                        if (!string.IsNullOrEmpty(parsed[0]))
                            Path = parsed[0];
                        Arguments = parsed[1];
                        ReferenceType = RefType.Powershell;
                    }
                    break;
                case RefType.Webpage:
                    SetBrowser();
                    if (!string.IsNullOrEmpty(Arguments) && Uri.TryCreate(Arguments, UriKind.Absolute, out _)) {
                        Path = Arguments;
                        Arguments = "";
                    }
                    break;
                case RefType.Powershell:
                    if (Path == "powershell.exe")
                        Path = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
                    if (Path == "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe" && !string.IsNullOrEmpty(Arguments)) {
                        var parsed = ExtractPSPathFromArgument(Arguments);
                        if (!string.IsNullOrEmpty(parsed[0]))
                            Path = parsed[0];
                        Arguments = parsed[1];
                    }
                    break;
                case RefType.File:

                    break;
                case RefType.Undetermined:
                case RefType.Folder:
                default:
                    return;
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
