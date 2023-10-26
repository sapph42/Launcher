
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
            this.BrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.CaptionTextBox = new System.Windows.Forms.TextBox();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.ArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Location of Program to Launch:";
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(245, 12);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse ...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Button Text";
            // 
            // CaptionTextBox
            // 
            this.CaptionTextBox.Location = new System.Drawing.Point(80, 120);
            this.CaptionTextBox.Name = "CaptionTextBox";
            this.CaptionTextBox.Size = new System.Drawing.Size(240, 20);
            this.CaptionTextBox.TabIndex = 4;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(15, 146);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 5;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(245, 146);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // PathTextBox
            // 
            this.PathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.PathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PathTextBox.Location = new System.Drawing.Point(15, 41);
            this.PathTextBox.Multiline = true;
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.Size = new System.Drawing.Size(305, 46);
            this.PathTextBox.TabIndex = 7;
            this.PathTextBox.TabStop = false;
            // 
            // ArgumentsTextBox
            // 
            this.ArgumentsTextBox.Location = new System.Drawing.Point(80, 93);
            this.ArgumentsTextBox.Name = "ArgumentsTextBox";
            this.ArgumentsTextBox.Size = new System.Drawing.Size(240, 20);
            this.ArgumentsTextBox.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Arguments";
            // 
            // CreateOrEditButton
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(329, 181);
            this.Controls.Add(this.ArgumentsTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PathTextBox);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.CaptionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.label1);
            this.Name = "CreateOrEditButton";
            this.Text = "Create New Launcher Button";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateButton_FormClosing);
            this.Load += new System.EventHandler(this.CreateButton_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CaptionTextBox;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.TextBox ArgumentsTextBox;
        private System.Windows.Forms.Label label3;
    }
}