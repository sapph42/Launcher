using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Launcher {
    public partial class CreateOrEditButton : Form {
        public string Path;
        public string Caption;
        public string Arguments;
        public bool AdminOnly;
        public Color Back;
        public LauncherButton.RefType ReferenceType;
        public LauncherButton.Browser TargetBrowser;
        public new DialogResult DialogResult = DialogResult.None;
        public System.Windows.Input.ModifierKeys KeyModifiers = System.Windows.Input.ModifierKeys.None;
        public Key HotkeyBase = Key.None;

        public CreateOrEditButton() : this("", "", default, "", false) { }
        public CreateOrEditButton(string path, string caption, Color back, string arguments = "", bool adminOnly = false) {
            Path = path;
            Caption = caption;
            Arguments = arguments;
            AdminOnly = adminOnly;
            InitializeComponent();
            Back = back;
        }
        public CreateOrEditButton(LauncherButton button) {
            Path = button.Path;
            Caption = button.Caption;
            Arguments = button.Arguments;
            AdminOnly = button.AdminOnly;
            Back = button.Background;
            ReferenceType = button.ReferenceType;
            TargetBrowser = button.TargetBrowser;
            if (button.HasHotKeySet) {
                KeyModifiers = button.KeyModifiers;
                HotkeyBase = button.KeyTarget;
            }
            InitializeComponent();
        }
        private string HotkeyDisplay (System.Windows.Input.ModifierKeys modifierKeys, Key? key) {
            StringBuilder sb = new StringBuilder();
            if ((modifierKeys & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
                sb.Append("CTRL + ");
            if ((modifierKeys & System.Windows.Input.ModifierKeys.Alt) == System.Windows.Input.ModifierKeys.Alt)
                sb.Append("ALT + ");
            if ((modifierKeys & System.Windows.Input.ModifierKeys.Shift) == System.Windows.Input.ModifierKeys.Shift)
                sb.Append("SHIFT + ");
            sb.Append(key?.ToString());
            return sb.ToString();
        }
        private string HotkeyDisplay (Keys keys) {
            StringBuilder sb = new StringBuilder();
            if ((keys & Keys.ControlKey) == Keys.ControlKey){
                sb.Append("CTRL + ");
                keys &= ~Keys.Control;
            }
            if ((keys & Keys.Alt) == Keys.Alt) {
                sb.Append("ALT + ");
                keys &= ~Keys.Alt;
            }
            if ((keys & Keys.ShiftKey) == Keys.ShiftKey) {
                sb.Append("SHIFT + ");
                keys &= ~Keys.Shift;
            }
            foreach (string key in keys.GetUniqueFlags().Cast<string>()) {
                sb.Append(key + " + ");
            }
            return sb.ToString();
        }
        private void CreateOrEditButton_Load(object sender, EventArgs e) {
            switch (ReferenceType) {
                case LauncherButton.RefType.Folder:
                    FolderPathTextBox.Text = Path ?? "";
                    FolderColorButton.BackColor = Back;
                    FolderCaptionTextBox.Text = Caption ?? "";
                    FolderHotkeyTextBox.Text = HotkeyDisplay(KeyModifiers, HotkeyBase);
                    Arguments = null;
                    AdminOnly = false;
                    TabControl.SelectedTab = FolderTab;
                    break;
                case LauncherButton.RefType.Webpage:
                    UriCaptionTextBox.Text = Caption ?? "";
                    UriColorButton.BackColor = Back;
                    UriHotkeyTextBox.Text = HotkeyDisplay(KeyModifiers, HotkeyBase);
                    TabControl.SelectedTab = WebTab;
                    bool typeChecked;
                    if (TargetBrowser != LauncherButton.Browser.None) {
                        typeChecked = true;
                        switch (TargetBrowser) {
                            case LauncherButton.Browser.Edge:
                                EdgeRadio.Checked = true;
                                break;
                            case LauncherButton.Browser.Chrome:
                                ChromeRadio.Checked = true;
                                break;
                            case LauncherButton.Browser.Firefox:
                                FirefoxRadio.Checked = true;
                                break;
                            default:
                                typeChecked = false;
                                break;
                        }
                        if (typeChecked) {
                            if (LauncherButton.BrowserPaths.Values.Contains(Path)) {
                                UriTargetTextBox.Text = Arguments;
                            } else {
                                UriTargetTextBox.Text = Path;
                            }
                            break; 
                        }
                    }
                    if (string.IsNullOrEmpty(Path)) {
                        UriTargetTextBox.Text = "";
                        EdgeRadio.Checked = true;
                        break;
                    }
                    if (Uri.IsWellFormedUriString(Path, UriKind.Absolute)) {
                        UriTargetTextBox.Text = Path;
                        EdgeRadio.Checked = true;
                        break;
                    }
                    switch (Path) {
                        case var PathVal when new Regex(@".*\\firefox\.exe").IsMatch(PathVal):
                            FirefoxRadio.Checked = true;
                            break;
                        case var PathVal when new Regex(@".*\\chrome\.exe").IsMatch(PathVal):
                            ChromeRadio.Checked = true;
                            break;
                        case "microsoft-edge:":
                        default:
                            EdgeRadio.Checked = true;
                            break;
                    }
                    UriTargetTextBox.Text = Arguments ?? "";
                    break;
                case LauncherButton.RefType.Powershell:
                    FileInfo PathInfo = new FileInfo(Path);
                    PowerShellColorButton.BackColor = Back;
                    PowerShellAdminCheck.Checked = AdminOnly;
                    PowerShellCaptionTextBox.Text = Caption;
                    PowerShellHotkeyTextBox.Text = HotkeyDisplay(KeyModifiers, HotkeyBase);
                    if (PathInfo.Extension == ".ps1") {
                        PowerShellTargetTextBox.Text = Path;
                        PowerShellArgumentsTextBox.Text = Arguments ?? "-ExecutionPolicy Bypass";
                    } else if (PathInfo.Name == "powershell.exe") {
                        Regex QuotedArgumentParser = new Regex(@"(?<startargs>.*)(?:-File\s)""(?<path>[^""]*)""\s(?<endargs>.*)");
                        Regex UnquotedArgumentParser = new Regex(@"(?<startargs>.*)(?:-File\s)(?<path>[^"" ]*)\s(?<endargs>.*)");
                        MatchCollection matches;
                        if (QuotedArgumentParser.IsMatch(Arguments)) {
                            matches = QuotedArgumentParser.Matches(Arguments);
                            var groups = matches[0].Groups;
                            PowerShellTargetTextBox.Text = groups["path"].Value;
                            if (!string.IsNullOrEmpty(groups["startargs"].Value)
                                && !string.IsNullOrEmpty(groups["endargs"].Value)) {
                                PowerShellArgumentsTextBox.Text = $"{groups["startargs"].Value} {groups["endargs"].Value}";
                            } else
                                PowerShellArgumentsTextBox.Text = groups["startargs"].Value + groups["endargs"].Value;
                        } else if (UnquotedArgumentParser.IsMatch(Arguments)) {
                            matches = UnquotedArgumentParser.Matches(Arguments);
                            var groups = matches[0].Groups;
                            PowerShellTargetTextBox.Text = groups["path"].Value;
                            if (!string.IsNullOrEmpty(groups["startargs"].Value)
                                && !string.IsNullOrEmpty(groups["endargs"].Value)) {
                                PowerShellArgumentsTextBox.Text = $"{groups["startargs"].Value} {groups["endargs"].Value}";
                            } else
                                PowerShellArgumentsTextBox.Text = groups["startargs"].Value ?? groups["endargs"].Value;
                        } else {
                            PowerShellTargetTextBox.Text = "";
                            PowerShellArgumentsTextBox.Text = Arguments;
                        }
                    }
                    TabControl.SelectedTab = PsTab;
                    break;
                case LauncherButton.RefType.File:
                    FileTargetTextBox.Text = Path ?? "";
                    FileCaptionTextBox.Text = Caption ?? "";
                    FileHotkeyTextBox.Text = HotkeyDisplay(KeyModifiers, HotkeyBase);
                    FileColorButton.BackColor = Back;
                    TabControl.SelectedTab = FileTab;
                    break;
                case LauncherButton.RefType.Undetermined:
                case LauncherButton.RefType.Program:
                default:
                    ProgramPathTextBox.Text = Path ?? "";
                    ProgramCaptionTextBox.Text = Caption ?? "";
                    ProgramArgumentsTextBox.Text = Arguments ?? "";
                    ProgramHotkeyTextBox.Text = HotkeyDisplay(KeyModifiers, HotkeyBase);
                    ProgramAdminCheck.Checked = AdminOnly;
                    ProgramColorButton.BackColor = Back;
                    TabControl.SelectedTab = ExeTab;
                    break;
            }
        }

        private void ProgramBrowseButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog() {
                InitialDirectory = @"C:\Program Files",
                Filter = "Executables (*.exe)|*.exe",
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK) {
                Path = dialog.FileName;
                ProgramPathTextBox.Text = Path;
            }
        }
        private void FolderBrowseButton_Click(object sender, EventArgs e) {

            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false,
                DefaultDirectory = string.IsNullOrEmpty(FolderPathTextBox.Text) ? @"C:\" : FolderPathTextBox.Text,
                InitialDirectory = string.IsNullOrEmpty(FolderPathTextBox.Text) ? @"C:\" : FolderPathTextBox.Text
            };
            if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok) {
                Path = dialog.FileName;
                ProgramPathTextBox.Text = Path;
            }
        }
        private void PowerShellBrowseButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog() {
                InitialDirectory = @"C:\Program Files",
                Filter = "PowerShell Scripts (*.ps1)|*.ps1",
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK) {
                Path = dialog.FileName;
                PowerShellTargetTextBox.Text = Path;
                if (PowerShellArgumentsTextBox.Text == "")
                    PowerShellArgumentsTextBox.Text = @"-ExecutionPolicy Bypass";
            }
        }
        private void FileBrowseButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog() {
                InitialDirectory = @"C:\Program Files",
                Filter = "All Files (*.*)|*.*",
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK) {
                Path = dialog.FileName;
                FileTargetTextBox.Text = Path;
            }
        }
        private void ProgramOkButton_Click(object sender, EventArgs e) {
            Caption = ProgramCaptionTextBox.Text;
            Arguments = ProgramArgumentsTextBox.Text;
            AdminOnly = ProgramAdminCheck.Checked;
            DialogResult = DialogResult.OK;
            Back = ProgramColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Program;
            Close();
        }
        private void FolderOkButton_Click(object sender, EventArgs e) {
            Caption = FolderCaptionTextBox.Text;
            Arguments = null;
            AdminOnly = false;
            DialogResult = DialogResult.OK;
            Back = FolderColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Folder;
            Close();
        }
        private void UriOkButton_Click(object sender, EventArgs e) {
            Caption = UriCaptionTextBox.Text;
            Arguments = "";
            if (EdgeRadio.Checked) {
                TargetBrowser = LauncherButton.Browser.Edge;
            } else if (FirefoxRadio.Checked) {
                TargetBrowser = LauncherButton.Browser.Firefox;
            } else {
                TargetBrowser = LauncherButton.Browser.Chrome;
            }
            Path = UriTargetTextBox.Text;
            AdminOnly = false;
            DialogResult = DialogResult.OK;
            Back = UriColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Webpage;
            Close();
        }
        private void PowerShellOkButton_Click(object sender, EventArgs e) {
            Caption = PowerShellCaptionTextBox.Text;
            Arguments = PowerShellArgumentsTextBox.Text;
            Path = PowerShellTargetTextBox.Text;
            AdminOnly = PowerShellAdminCheck.Checked;
            DialogResult = DialogResult.OK;
            Back = PowerShellColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Powershell;
            Close();
        }
        private void FileOkButton_Click(object sender, EventArgs e) {
            Caption = FileCaptionTextBox.Text;
            Path = FileTargetTextBox.Text;
            DialogResult = DialogResult.OK;
            Back = FileColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.File;
            Close();
        }
        private void CancelButton_Click(object sender, EventArgs e) {
            Path = string.Empty;
            Caption = string.Empty;
            Arguments = string.Empty;
            AdminOnly = false;
            KeyModifiers = System.Windows.Input.ModifierKeys.None;
            HotkeyBase = Key.None;
            Back = SystemColors.Control;
            DialogResult = DialogResult.Cancel;
            ReferenceType = LauncherButton.RefType.Undetermined;
            Close();
        }
        private void CreateButton_FormClosing(object sender, FormClosingEventArgs e) {
            if (DialogResult != DialogResult.None) return;
            Path = string.Empty;
            Caption = string.Empty;
            Arguments = string.Empty;
            AdminOnly = false;
            KeyModifiers = System.Windows.Input.ModifierKeys.None;
            HotkeyBase = Key.None;
            Back = SystemColors.Control;
            DialogResult = DialogResult.Cancel;
            ReferenceType = LauncherButton.RefType.Undetermined;
        }
        private void ColorButton_Click(object sender, EventArgs e) {
            Button button = (Button)sender;
            ColorDialog colorDialog = new ColorDialog {
                Color = Back
            };
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                Back = colorDialog.Color;
                button.BackColor = colorDialog.Color;
            }
        }
        private void HotkeyTextBox_Enter(object sender, EventArgs e) {
            KeyPreview = false;
        }
        private void HotkeyTextBox_Leave(object sender, EventArgs e) {
            KeyPreview = true;
        }
        private void HotkeyTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            TextBox source = sender as TextBox;
            if (e.KeyCode == Keys.Escape) {
                source.Text = "";
                KeyModifiers = System.Windows.Input.ModifierKeys.None;
                HotkeyBase = Key.None;
                e.Handled = true;
            }
            Keys k = e.KeyData;
            Keys lastKey = e.KeyCode;
            if (e.Control) {
                k &= ~(Keys.Control | Keys.ControlKey | Keys.LControlKey | Keys.RControlKey);
                KeyModifiers |= System.Windows.Input.ModifierKeys.Control;
                if (lastKey == Keys.ControlKey)
                    lastKey = Keys.None;
            }
            if (e.Shift) {
                k &= ~(Keys.Shift | Keys.ShiftKey | Keys.LShiftKey | Keys.RShiftKey);
                KeyModifiers |= System.Windows.Input.ModifierKeys.Shift;
                if (lastKey == Keys.ShiftKey)
                    lastKey = Keys.None;
            }
            if (e.Alt) {
                k &= ~(Keys.Alt | Keys.Menu | Keys.LMenu | Keys.RMenu);
                KeyModifiers |= System.Windows.Input.ModifierKeys.Alt;
                if (lastKey == Keys.Alt || lastKey == Keys.Menu)
                    lastKey = Keys.None;
            }
            int v = (int)k;
            HotkeyBase = KeyInterop.KeyFromVirtualKey((int)lastKey);
            source.Text = (e.Control ? "[Ctrl] + " : "") + (e.Alt ? "[Alt] + " : "") + (e.Shift ? "[Shift] + " : "") + (lastKey == Keys.None ? "" : lastKey.ToString());
            e.Handled = true;
        }
    }
}
