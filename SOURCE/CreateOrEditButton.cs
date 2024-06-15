using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    public partial class CreateOrEditButton : Form {
        public string Path;
        public string Caption;
        public string Arguments;
        public bool AdminOnly;
        public Color Back;
        public LauncherButton.RefType ReferenceType;
        public new DialogResult DialogResult = DialogResult.None;

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
            InitializeComponent();
        }

        private void CreateOrEditButton_Load(object sender, EventArgs e) {
            switch (ReferenceType) {
                case LauncherButton.RefType.Folder:
                    FolderPathTextBox.Text = Path ?? "";
                    FolderColorButton.BackColor = Back;
                    FolderCaptionTextBox.Text = Caption ?? "";
                    Arguments = null;
                    AdminOnly = false;
                    TabControl.SelectedTab = FolderTab;
                    break;
                case LauncherButton.RefType.Webpage:
                    UriCaptionTextBox.Text = Caption ?? "";
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
                    UriColorButton.BackColor = Back;
                    TabControl.SelectedTab = WebTab;
                    break;
                case LauncherButton.RefType.Powershell:
                    FileInfo PathInfo = new FileInfo(Path);
                    PowerShellColorButton.BackColor = Back;
                    PowerShellAdminCheck.Checked = AdminOnly;
                    if (PathInfo.Extension == ".ps1") {
                        Path = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
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
                case LauncherButton.RefType.Undetermined:
                case LauncherButton.RefType.Program:
                default:
                    ProgramPathTextBox.Text = Path ?? "";
                    ProgramCaptionTextBox.Text = Caption ?? "";
                    ProgramArgumentsTextBox.Text = Arguments ?? "";
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
            Arguments = UriTargetTextBox.Text;
            if (EdgeRadio.Checked) {
                Path = "microsoft-edge:";
            } else if (FirefoxRadio.Checked) {
                Path = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            } else {
                Path = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            }
            AdminOnly = false;
            DialogResult = DialogResult.OK;
            Back = UriColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Webpage;
            Close();
        }
        private void PowerShellOkButton_Click(object sender, EventArgs e) {
            Caption = ProgramCaptionTextBox.Text;
            Arguments = $@"-File ""{PowerShellTargetTextBox.Text}"" {ProgramArgumentsTextBox.Text}";
            Path = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            AdminOnly = ProgramAdminCheck.Checked;
            DialogResult = DialogResult.OK;
            Back = ProgramColorButton.BackColor;
            ReferenceType = LauncherButton.RefType.Program;
            Close();
        }
        private void CancelButton_Click(object sender, EventArgs e) {
            Path = string.Empty;
            Caption = string.Empty;
            Arguments = string.Empty;
            AdminOnly = false;
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
    }
}
