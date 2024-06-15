
namespace Launcher {
    partial class CreateOrEditButton {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.ProgramBrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ProgramCaptionTextBox = new System.Windows.Forms.TextBox();
            this.ProgramOkButton = new System.Windows.Forms.Button();
            this.ProgramCancelButton = new System.Windows.Forms.Button();
            this.ProgramPathTextBox = new System.Windows.Forms.TextBox();
            this.ProgramArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ProgramAdminCheck = new System.Windows.Forms.CheckBox();
            this.ProgramColorButton = new System.Windows.Forms.Button();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.ExeTab = new System.Windows.Forms.TabPage();
            this.FolderTab = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.FolderOkButton = new System.Windows.Forms.Button();
            this.FolderCancelButton = new System.Windows.Forms.Button();
            this.FolderColorButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.FolderCaptionTextBox = new System.Windows.Forms.TextBox();
            this.FolderBrowseButton = new System.Windows.Forms.Button();
            this.FolderPathTextBox = new System.Windows.Forms.TextBox();
            this.WebTab = new System.Windows.Forms.TabPage();
            this.UriColorButton = new System.Windows.Forms.Button();
            this.ChromeRadio = new System.Windows.Forms.RadioButton();
            this.FirefoxRadio = new System.Windows.Forms.RadioButton();
            this.EdgeRadio = new System.Windows.Forms.RadioButton();
            this.UriTargetTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.UriOkButton = new System.Windows.Forms.Button();
            this.UriCancelButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.UriCaptionTextBox = new System.Windows.Forms.TextBox();
            this.PsTab = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.PowerShellOkButton = new System.Windows.Forms.Button();
            this.PowerShellCancelButton = new System.Windows.Forms.Button();
            this.PowerShellArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.PowerShellColorButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.PowerShellCaptionTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.PowerShellBrowseButton = new System.Windows.Forms.Button();
            this.PowerShellAdminCheck = new System.Windows.Forms.CheckBox();
            this.PowerShellTargetTextBox = new System.Windows.Forms.TextBox();
            this.TabControl.SuspendLayout();
            this.ExeTab.SuspendLayout();
            this.FolderTab.SuspendLayout();
            this.WebTab.SuspendLayout();
            this.PsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Location of Program to Launch:";
            // 
            // ProgramBrowseButton
            // 
            this.ProgramBrowseButton.Location = new System.Drawing.Point(293, 6);
            this.ProgramBrowseButton.Name = "ProgramBrowseButton";
            this.ProgramBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.ProgramBrowseButton.TabIndex = 1;
            this.ProgramBrowseButton.Text = "Browse ...";
            this.ProgramBrowseButton.UseVisualStyleBackColor = true;
            this.ProgramBrowseButton.Click += new System.EventHandler(this.ProgramBrowseButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Button Text";
            // 
            // ProgramCaptionTextBox
            // 
            this.ProgramCaptionTextBox.Location = new System.Drawing.Point(128, 134);
            this.ProgramCaptionTextBox.Name = "ProgramCaptionTextBox";
            this.ProgramCaptionTextBox.Size = new System.Drawing.Size(240, 20);
            this.ProgramCaptionTextBox.TabIndex = 4;
            // 
            // ProgramOkButton
            // 
            this.ProgramOkButton.Location = new System.Drawing.Point(9, 172);
            this.ProgramOkButton.Name = "ProgramOkButton";
            this.ProgramOkButton.Size = new System.Drawing.Size(75, 23);
            this.ProgramOkButton.TabIndex = 5;
            this.ProgramOkButton.Text = "OK";
            this.ProgramOkButton.UseVisualStyleBackColor = true;
            this.ProgramOkButton.Click += new System.EventHandler(this.ProgramOkButton_Click);
            // 
            // ProgramCancelButton
            // 
            this.ProgramCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ProgramCancelButton.Location = new System.Drawing.Point(293, 172);
            this.ProgramCancelButton.Name = "ProgramCancelButton";
            this.ProgramCancelButton.Size = new System.Drawing.Size(75, 23);
            this.ProgramCancelButton.TabIndex = 6;
            this.ProgramCancelButton.Text = "Cancel";
            this.ProgramCancelButton.UseVisualStyleBackColor = true;
            this.ProgramCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ProgramPathTextBox
            // 
            this.ProgramPathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ProgramPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ProgramPathTextBox.Location = new System.Drawing.Point(9, 29);
            this.ProgramPathTextBox.Multiline = true;
            this.ProgramPathTextBox.Name = "ProgramPathTextBox";
            this.ProgramPathTextBox.Size = new System.Drawing.Size(359, 46);
            this.ProgramPathTextBox.TabIndex = 7;
            this.ProgramPathTextBox.TabStop = false;
            // 
            // ProgramArgumentsTextBox
            // 
            this.ProgramArgumentsTextBox.Location = new System.Drawing.Point(128, 108);
            this.ProgramArgumentsTextBox.Name = "ProgramArgumentsTextBox";
            this.ProgramArgumentsTextBox.Size = new System.Drawing.Size(240, 20);
            this.ProgramArgumentsTextBox.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Arguments";
            // 
            // ProgramAdminCheck
            // 
            this.ProgramAdminCheck.AutoSize = true;
            this.ProgramAdminCheck.Location = new System.Drawing.Point(9, 81);
            this.ProgramAdminCheck.Name = "ProgramAdminCheck";
            this.ProgramAdminCheck.Size = new System.Drawing.Size(94, 17);
            this.ProgramAdminCheck.TabIndex = 10;
            this.ProgramAdminCheck.Text = "Always RunAs";
            this.ProgramAdminCheck.UseVisualStyleBackColor = true;
            // 
            // ProgramColorButton
            // 
            this.ProgramColorButton.Location = new System.Drawing.Point(345, 81);
            this.ProgramColorButton.Name = "ProgramColorButton";
            this.ProgramColorButton.Size = new System.Drawing.Size(23, 21);
            this.ProgramColorButton.TabIndex = 11;
            this.ProgramColorButton.UseVisualStyleBackColor = true;
            this.ProgramColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.ExeTab);
            this.TabControl.Controls.Add(this.FolderTab);
            this.TabControl.Controls.Add(this.WebTab);
            this.TabControl.Controls.Add(this.PsTab);
            this.TabControl.Location = new System.Drawing.Point(12, 12);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(382, 227);
            this.TabControl.TabIndex = 12;
            // 
            // ExeTab
            // 
            this.ExeTab.Controls.Add(this.label1);
            this.ExeTab.Controls.Add(this.ProgramOkButton);
            this.ExeTab.Controls.Add(this.ProgramCancelButton);
            this.ExeTab.Controls.Add(this.ProgramArgumentsTextBox);
            this.ExeTab.Controls.Add(this.ProgramColorButton);
            this.ExeTab.Controls.Add(this.label2);
            this.ExeTab.Controls.Add(this.ProgramCaptionTextBox);
            this.ExeTab.Controls.Add(this.label3);
            this.ExeTab.Controls.Add(this.ProgramBrowseButton);
            this.ExeTab.Controls.Add(this.ProgramAdminCheck);
            this.ExeTab.Controls.Add(this.ProgramPathTextBox);
            this.ExeTab.Location = new System.Drawing.Point(4, 22);
            this.ExeTab.Name = "ExeTab";
            this.ExeTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExeTab.Size = new System.Drawing.Size(374, 201);
            this.ExeTab.TabIndex = 0;
            this.ExeTab.Text = "Programs";
            this.ExeTab.UseVisualStyleBackColor = true;
            // 
            // FolderTab
            // 
            this.FolderTab.Controls.Add(this.label4);
            this.FolderTab.Controls.Add(this.FolderOkButton);
            this.FolderTab.Controls.Add(this.FolderCancelButton);
            this.FolderTab.Controls.Add(this.FolderColorButton);
            this.FolderTab.Controls.Add(this.label5);
            this.FolderTab.Controls.Add(this.FolderCaptionTextBox);
            this.FolderTab.Controls.Add(this.FolderBrowseButton);
            this.FolderTab.Controls.Add(this.FolderPathTextBox);
            this.FolderTab.Location = new System.Drawing.Point(4, 22);
            this.FolderTab.Name = "FolderTab";
            this.FolderTab.Padding = new System.Windows.Forms.Padding(3);
            this.FolderTab.Size = new System.Drawing.Size(374, 201);
            this.FolderTab.TabIndex = 1;
            this.FolderTab.Text = "Folders";
            this.FolderTab.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Location of Folder to Open";
            // 
            // FolderOkButton
            // 
            this.FolderOkButton.Location = new System.Drawing.Point(9, 172);
            this.FolderOkButton.Name = "FolderOkButton";
            this.FolderOkButton.Size = new System.Drawing.Size(75, 23);
            this.FolderOkButton.TabIndex = 16;
            this.FolderOkButton.Text = "OK";
            this.FolderOkButton.UseVisualStyleBackColor = true;
            this.FolderOkButton.Click += new System.EventHandler(this.FolderOkButton_Click);
            // 
            // FolderCancelButton
            // 
            this.FolderCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.FolderCancelButton.Location = new System.Drawing.Point(293, 172);
            this.FolderCancelButton.Name = "FolderCancelButton";
            this.FolderCancelButton.Size = new System.Drawing.Size(75, 23);
            this.FolderCancelButton.TabIndex = 17;
            this.FolderCancelButton.Text = "Cancel";
            this.FolderCancelButton.UseVisualStyleBackColor = true;
            this.FolderCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // FolderColorButton
            // 
            this.FolderColorButton.Location = new System.Drawing.Point(345, 81);
            this.FolderColorButton.Name = "FolderColorButton";
            this.FolderColorButton.Size = new System.Drawing.Size(23, 21);
            this.FolderColorButton.TabIndex = 19;
            this.FolderColorButton.UseVisualStyleBackColor = true;
            this.FolderColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Button Text";
            // 
            // FolderCaptionTextBox
            // 
            this.FolderCaptionTextBox.Location = new System.Drawing.Point(128, 134);
            this.FolderCaptionTextBox.Name = "FolderCaptionTextBox";
            this.FolderCaptionTextBox.Size = new System.Drawing.Size(240, 20);
            this.FolderCaptionTextBox.TabIndex = 15;
            // 
            // FolderBrowseButton
            // 
            this.FolderBrowseButton.Location = new System.Drawing.Point(293, 6);
            this.FolderBrowseButton.Name = "FolderBrowseButton";
            this.FolderBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.FolderBrowseButton.TabIndex = 13;
            this.FolderBrowseButton.Text = "Browse ...";
            this.FolderBrowseButton.UseVisualStyleBackColor = true;
            this.FolderBrowseButton.Click += new System.EventHandler(this.FolderBrowseButton_Click);
            // 
            // FolderPathTextBox
            // 
            this.FolderPathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.FolderPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FolderPathTextBox.Location = new System.Drawing.Point(9, 29);
            this.FolderPathTextBox.Multiline = true;
            this.FolderPathTextBox.Name = "FolderPathTextBox";
            this.FolderPathTextBox.Size = new System.Drawing.Size(359, 46);
            this.FolderPathTextBox.TabIndex = 18;
            this.FolderPathTextBox.TabStop = false;
            // 
            // WebTab
            // 
            this.WebTab.Controls.Add(this.UriColorButton);
            this.WebTab.Controls.Add(this.ChromeRadio);
            this.WebTab.Controls.Add(this.FirefoxRadio);
            this.WebTab.Controls.Add(this.EdgeRadio);
            this.WebTab.Controls.Add(this.UriTargetTextBox);
            this.WebTab.Controls.Add(this.label6);
            this.WebTab.Controls.Add(this.UriOkButton);
            this.WebTab.Controls.Add(this.UriCancelButton);
            this.WebTab.Controls.Add(this.label7);
            this.WebTab.Controls.Add(this.UriCaptionTextBox);
            this.WebTab.Location = new System.Drawing.Point(4, 22);
            this.WebTab.Name = "WebTab";
            this.WebTab.Padding = new System.Windows.Forms.Padding(3);
            this.WebTab.Size = new System.Drawing.Size(374, 201);
            this.WebTab.TabIndex = 2;
            this.WebTab.Text = "WebPage";
            this.WebTab.UseVisualStyleBackColor = true;
            // 
            // UriColorButton
            // 
            this.UriColorButton.Location = new System.Drawing.Point(345, 93);
            this.UriColorButton.Name = "UriColorButton";
            this.UriColorButton.Size = new System.Drawing.Size(23, 21);
            this.UriColorButton.TabIndex = 20;
            this.UriColorButton.UseVisualStyleBackColor = true;
            this.UriColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // ChromeRadio
            // 
            this.ChromeRadio.AutoSize = true;
            this.ChromeRadio.Location = new System.Drawing.Point(307, 120);
            this.ChromeRadio.Name = "ChromeRadio";
            this.ChromeRadio.Size = new System.Drawing.Size(61, 17);
            this.ChromeRadio.TabIndex = 16;
            this.ChromeRadio.TabStop = true;
            this.ChromeRadio.Text = "Chrome";
            this.ChromeRadio.UseVisualStyleBackColor = true;
            // 
            // FirefoxRadio
            // 
            this.FirefoxRadio.AutoSize = true;
            this.FirefoxRadio.Location = new System.Drawing.Point(158, 120);
            this.FirefoxRadio.Name = "FirefoxRadio";
            this.FirefoxRadio.Size = new System.Drawing.Size(56, 17);
            this.FirefoxRadio.TabIndex = 15;
            this.FirefoxRadio.TabStop = true;
            this.FirefoxRadio.Text = "Firefox";
            this.FirefoxRadio.UseVisualStyleBackColor = true;
            // 
            // EdgeRadio
            // 
            this.EdgeRadio.AutoSize = true;
            this.EdgeRadio.Location = new System.Drawing.Point(9, 120);
            this.EdgeRadio.Name = "EdgeRadio";
            this.EdgeRadio.Size = new System.Drawing.Size(50, 17);
            this.EdgeRadio.TabIndex = 14;
            this.EdgeRadio.TabStop = true;
            this.EdgeRadio.Text = "Edge";
            this.EdgeRadio.UseVisualStyleBackColor = true;
            // 
            // UriTargetTextBox
            // 
            this.UriTargetTextBox.Location = new System.Drawing.Point(9, 22);
            this.UriTargetTextBox.Multiline = true;
            this.UriTargetTextBox.Name = "UriTargetTextBox";
            this.UriTargetTextBox.Size = new System.Drawing.Size(359, 65);
            this.UriTargetTextBox.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Target URI:";
            // 
            // UriOkButton
            // 
            this.UriOkButton.Location = new System.Drawing.Point(9, 171);
            this.UriOkButton.Name = "UriOkButton";
            this.UriOkButton.Size = new System.Drawing.Size(75, 23);
            this.UriOkButton.TabIndex = 11;
            this.UriOkButton.Text = "OK";
            this.UriOkButton.UseVisualStyleBackColor = true;
            this.UriOkButton.Click += new System.EventHandler(this.UriOkButton_Click);
            // 
            // UriCancelButton
            // 
            this.UriCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.UriCancelButton.Location = new System.Drawing.Point(293, 171);
            this.UriCancelButton.Name = "UriCancelButton";
            this.UriCancelButton.Size = new System.Drawing.Size(75, 23);
            this.UriCancelButton.TabIndex = 12;
            this.UriCancelButton.Text = "Cancel";
            this.UriCancelButton.UseVisualStyleBackColor = true;
            this.UriCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Button Text";
            // 
            // UriCaptionTextBox
            // 
            this.UriCaptionTextBox.Location = new System.Drawing.Point(128, 143);
            this.UriCaptionTextBox.Name = "UriCaptionTextBox";
            this.UriCaptionTextBox.Size = new System.Drawing.Size(240, 20);
            this.UriCaptionTextBox.TabIndex = 10;
            // 
            // PsTab
            // 
            this.PsTab.Controls.Add(this.label8);
            this.PsTab.Controls.Add(this.PowerShellOkButton);
            this.PsTab.Controls.Add(this.PowerShellCancelButton);
            this.PsTab.Controls.Add(this.PowerShellArgumentsTextBox);
            this.PsTab.Controls.Add(this.PowerShellColorButton);
            this.PsTab.Controls.Add(this.label9);
            this.PsTab.Controls.Add(this.PowerShellCaptionTextBox);
            this.PsTab.Controls.Add(this.label10);
            this.PsTab.Controls.Add(this.PowerShellBrowseButton);
            this.PsTab.Controls.Add(this.PowerShellAdminCheck);
            this.PsTab.Controls.Add(this.PowerShellTargetTextBox);
            this.PsTab.Location = new System.Drawing.Point(4, 22);
            this.PsTab.Name = "PsTab";
            this.PsTab.Padding = new System.Windows.Forms.Padding(3);
            this.PsTab.Size = new System.Drawing.Size(374, 201);
            this.PsTab.TabIndex = 3;
            this.PsTab.Text = "PowerShell";
            this.PsTab.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Location of PS Script to Launch:";
            // 
            // PowerShellOkButton
            // 
            this.PowerShellOkButton.Location = new System.Drawing.Point(9, 172);
            this.PowerShellOkButton.Name = "PowerShellOkButton";
            this.PowerShellOkButton.Size = new System.Drawing.Size(75, 23);
            this.PowerShellOkButton.TabIndex = 16;
            this.PowerShellOkButton.Text = "OK";
            this.PowerShellOkButton.UseVisualStyleBackColor = true;
            this.PowerShellOkButton.Click += new System.EventHandler(this.UriOkButton_Click);
            // 
            // PowerShellCancelButton
            // 
            this.PowerShellCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.PowerShellCancelButton.Location = new System.Drawing.Point(293, 172);
            this.PowerShellCancelButton.Name = "PowerShellCancelButton";
            this.PowerShellCancelButton.Size = new System.Drawing.Size(75, 23);
            this.PowerShellCancelButton.TabIndex = 17;
            this.PowerShellCancelButton.Text = "Cancel";
            this.PowerShellCancelButton.UseVisualStyleBackColor = true;
            this.PowerShellCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // PowerShellArgumentsTextBox
            // 
            this.PowerShellArgumentsTextBox.Location = new System.Drawing.Point(128, 108);
            this.PowerShellArgumentsTextBox.Name = "PowerShellArgumentsTextBox";
            this.PowerShellArgumentsTextBox.Size = new System.Drawing.Size(240, 20);
            this.PowerShellArgumentsTextBox.TabIndex = 20;
            // 
            // PowerShellColorButton
            // 
            this.PowerShellColorButton.Location = new System.Drawing.Point(345, 81);
            this.PowerShellColorButton.Name = "PowerShellColorButton";
            this.PowerShellColorButton.Size = new System.Drawing.Size(23, 21);
            this.PowerShellColorButton.TabIndex = 22;
            this.PowerShellColorButton.UseVisualStyleBackColor = true;
            this.PowerShellColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 137);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Button Text";
            // 
            // PowerShellCaptionTextBox
            // 
            this.PowerShellCaptionTextBox.Location = new System.Drawing.Point(128, 134);
            this.PowerShellCaptionTextBox.Name = "PowerShellCaptionTextBox";
            this.PowerShellCaptionTextBox.Size = new System.Drawing.Size(240, 20);
            this.PowerShellCaptionTextBox.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 111);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Arguments";
            // 
            // PowerShellBrowseButton
            // 
            this.PowerShellBrowseButton.Location = new System.Drawing.Point(293, 6);
            this.PowerShellBrowseButton.Name = "PowerShellBrowseButton";
            this.PowerShellBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.PowerShellBrowseButton.TabIndex = 13;
            this.PowerShellBrowseButton.Text = "Browse ...";
            this.PowerShellBrowseButton.UseVisualStyleBackColor = true;
            this.PowerShellBrowseButton.Click += new System.EventHandler(this.PowerShellBrowseButton_Click);
            // 
            // PowerShellAdminCheck
            // 
            this.PowerShellAdminCheck.AutoSize = true;
            this.PowerShellAdminCheck.Location = new System.Drawing.Point(9, 81);
            this.PowerShellAdminCheck.Name = "PowerShellAdminCheck";
            this.PowerShellAdminCheck.Size = new System.Drawing.Size(94, 17);
            this.PowerShellAdminCheck.TabIndex = 21;
            this.PowerShellAdminCheck.Text = "Always RunAs";
            this.PowerShellAdminCheck.UseVisualStyleBackColor = true;
            // 
            // PowerShellTargetTextBox
            // 
            this.PowerShellTargetTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.PowerShellTargetTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PowerShellTargetTextBox.Location = new System.Drawing.Point(9, 29);
            this.PowerShellTargetTextBox.Multiline = true;
            this.PowerShellTargetTextBox.Name = "PowerShellTargetTextBox";
            this.PowerShellTargetTextBox.Size = new System.Drawing.Size(359, 46);
            this.PowerShellTargetTextBox.TabIndex = 18;
            this.PowerShellTargetTextBox.TabStop = false;
            // 
            // CreateOrEditButton
            // 
            this.AcceptButton = this.ProgramOkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ProgramCancelButton;
            this.ClientSize = new System.Drawing.Size(401, 249);
            this.Controls.Add(this.TabControl);
            this.Name = "CreateOrEditButton";
            this.Text = "Create New Launcher Button";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateButton_FormClosing);
            this.Load += new System.EventHandler(this.CreateOrEditButton_Load);
            this.TabControl.ResumeLayout(false);
            this.ExeTab.ResumeLayout(false);
            this.ExeTab.PerformLayout();
            this.FolderTab.ResumeLayout(false);
            this.FolderTab.PerformLayout();
            this.WebTab.ResumeLayout(false);
            this.WebTab.PerformLayout();
            this.PsTab.ResumeLayout(false);
            this.PsTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ProgramBrowseButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ProgramCaptionTextBox;
        private System.Windows.Forms.Button ProgramOkButton;
        private System.Windows.Forms.Button ProgramCancelButton;
        private System.Windows.Forms.TextBox ProgramPathTextBox;
        private System.Windows.Forms.TextBox ProgramArgumentsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ProgramAdminCheck;
        private System.Windows.Forms.Button ProgramColorButton;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage ExeTab;
        private System.Windows.Forms.TabPage FolderTab;
        private System.Windows.Forms.TabPage WebTab;
        private System.Windows.Forms.TabPage PsTab;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button FolderOkButton;
        private System.Windows.Forms.Button FolderCancelButton;
        private System.Windows.Forms.Button FolderColorButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox FolderCaptionTextBox;
        private System.Windows.Forms.Button FolderBrowseButton;
        private System.Windows.Forms.TextBox FolderPathTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button UriOkButton;
        private System.Windows.Forms.Button UriCancelButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox UriCaptionTextBox;
        private System.Windows.Forms.RadioButton ChromeRadio;
        private System.Windows.Forms.RadioButton FirefoxRadio;
        private System.Windows.Forms.RadioButton EdgeRadio;
        private System.Windows.Forms.TextBox UriTargetTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button PowerShellOkButton;
        private System.Windows.Forms.Button PowerShellCancelButton;
        private System.Windows.Forms.TextBox PowerShellArgumentsTextBox;
        private System.Windows.Forms.Button PowerShellColorButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PowerShellCaptionTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button PowerShellBrowseButton;
        private System.Windows.Forms.CheckBox PowerShellAdminCheck;
        private System.Windows.Forms.TextBox PowerShellTargetTextBox;
        private System.Windows.Forms.Button UriColorButton;
    }
}