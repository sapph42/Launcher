using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    public partial class CreateOrEditButton : Form {
        public string Path;
        public string Caption;
        public new DialogResult DialogResult = DialogResult.None;

        public CreateOrEditButton() : this("", "") { }

        public CreateOrEditButton(string path, string caption) {
            Path = path;
            Caption = caption;
            InitializeComponent();
        }

        private void CreateButton_Load(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(Path))
                PathTextBox.Text = Path;
            if (!string.IsNullOrEmpty(Caption))
                CaptionTextBox.Text = Caption;
        }

        private void BrowseButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog() {
                InitialDirectory = @"C:\Program Files",
                Filter = "Executables (*.exe)|*.exe|PowerShell Scripts (*.ps1)|*.ps1|All files (*.*)|*.*",
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK) {
                Path = dialog.FileName;
                PathTextBox.Text = Path;
            }
        }

        private void OK_Click(object sender, EventArgs e) {
            Caption = CaptionTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e) {
            Path = string.Empty;
            Caption = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CreateButton_FormClosing(object sender, FormClosingEventArgs e) {
            if (DialogResult != DialogResult.None) return;
            Path = string.Empty;
            Caption = string.Empty;
            DialogResult = DialogResult.Cancel;
        }

    }
}
