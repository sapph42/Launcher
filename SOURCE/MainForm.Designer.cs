﻿
namespace Launcher {
    partial class MainForm {
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.standardPage = new System.Windows.Forms.TabPage();
            this.adminPage = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.standardPage);
            this.tabControl.Controls.Add(this.adminPage);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(114, 90);
            this.tabControl.TabIndex = 0;
            // 
            // standardPage
            // 
            this.standardPage.Location = new System.Drawing.Point(4, 22);
            this.standardPage.Name = "standardPage";
            this.standardPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.standardPage.Size = new System.Drawing.Size(106, 64);
            this.standardPage.TabIndex = 0;
            this.standardPage.Text = "Standard";
            this.standardPage.UseVisualStyleBackColor = true;
            // 
            // adminPage
            // 
            this.adminPage.Location = new System.Drawing.Point(4, 22);
            this.adminPage.Name = "adminPage";
            this.adminPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.adminPage.Size = new System.Drawing.Size(106, 64);
            this.adminPage.TabIndex = 1;
            this.adminPage.Text = "Admin";
            this.adminPage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(148, 114);
            this.Controls.Add(this.tabControl);
            this.Name = "MainForm";
            this.Text = "Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage standardPage;
        private System.Windows.Forms.TabPage adminPage;
    }
}

