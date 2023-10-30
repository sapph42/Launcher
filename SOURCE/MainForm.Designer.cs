
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RearrangeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.gridSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddRowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AddColumnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEmptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveRowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveColumnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AddRow = new System.Windows.Forms.PictureBox();
            this.RemoveRow = new System.Windows.Forms.PictureBox();
            this.RemoveColumn = new System.Windows.Forms.PictureBox();
            this.AddColumn = new System.Windows.Forms.PictureBox();
            this.RightClickMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AddRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RemoveRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RemoveColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AddColumn)).BeginInit();
            this.SuspendLayout();
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.editToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.RightClickMenu.Name = "RightClickMenu";
            this.RightClickMenu.Size = new System.Drawing.Size(109, 76);
            // 
            // runAsToolStripMenuItem
            // 
            this.runAsToolStripMenuItem.Name = "runAsToolStripMenuItem";
            this.runAsToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.runAsToolStripMenuItem.Text = "RunAs";
            this.runAsToolStripMenuItem.Click += new System.EventHandler(this.RunAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(105, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editLayoutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(148, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editLayoutToolStripMenuItem
            // 
            this.editLayoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RearrangeMenu,
            this.gridSizeToolStripMenuItem,
            this.ExitMenu});
            this.editLayoutToolStripMenuItem.Name = "editLayoutToolStripMenuItem";
            this.editLayoutToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.editLayoutToolStripMenuItem.Text = "Edit Layout";
            // 
            // RearrangeMenu
            // 
            this.RearrangeMenu.Name = "RearrangeMenu";
            this.RearrangeMenu.Size = new System.Drawing.Size(199, 22);
            this.RearrangeMenu.Text = "Toggle Rearrange Mode";
            this.RearrangeMenu.Click += new System.EventHandler(this.RearrangeMenu_Click);
            // 
            // gridSizeToolStripMenuItem
            // 
            this.gridSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeEmptyToolStripMenuItem});
            this.gridSizeToolStripMenuItem.Name = "gridSizeToolStripMenuItem";
            this.gridSizeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.gridSizeToolStripMenuItem.Text = "Grid Size";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddRowMenu,
            this.AddColumnMenu});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // AddRowMenu
            // 
            this.AddRowMenu.Name = "AddRowMenu";
            this.AddRowMenu.Size = new System.Drawing.Size(117, 22);
            this.AddRowMenu.Text = "Row";
            this.AddRowMenu.Click += new System.EventHandler(this.DoAddRow);
            // 
            // AddColumnMenu
            // 
            this.AddColumnMenu.Name = "AddColumnMenu";
            this.AddColumnMenu.Size = new System.Drawing.Size(117, 22);
            this.AddColumnMenu.Text = "Column";
            this.AddColumnMenu.Click += new System.EventHandler(this.DoAddColumn);
            // 
            // removeEmptyToolStripMenuItem
            // 
            this.removeEmptyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveRowMenu,
            this.RemoveColumnMenu});
            this.removeEmptyToolStripMenuItem.Name = "removeEmptyToolStripMenuItem";
            this.removeEmptyToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.removeEmptyToolStripMenuItem.Text = "Remove Empty";
            // 
            // RemoveRowMenu
            // 
            this.RemoveRowMenu.Name = "RemoveRowMenu";
            this.RemoveRowMenu.Size = new System.Drawing.Size(117, 22);
            this.RemoveRowMenu.Text = "Row";
            this.RemoveRowMenu.Click += new System.EventHandler(this.DoRemoveRow);
            // 
            // RemoveColumnMenu
            // 
            this.RemoveColumnMenu.Name = "RemoveColumnMenu";
            this.RemoveColumnMenu.Size = new System.Drawing.Size(117, 22);
            this.RemoveColumnMenu.Text = "Column";
            this.RemoveColumnMenu.Click += new System.EventHandler(this.DoRemoveColumn);
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(199, 22);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
            // 
            // AddRow
            // 
            this.AddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddRow.Image = global::Launcher.Properties.Resources.Add;
            this.AddRow.Location = new System.Drawing.Point(7, 95);
            this.AddRow.Name = "AddRow";
            this.AddRow.Size = new System.Drawing.Size(12, 12);
            this.AddRow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AddRow.TabIndex = 4;
            this.AddRow.TabStop = false;
            this.AddRow.Click += new System.EventHandler(this.DoAddRow);
            // 
            // RemoveRow
            // 
            this.RemoveRow.Image = global::Launcher.Properties.Resources.Remove;
            this.RemoveRow.Location = new System.Drawing.Point(7, 76);
            this.RemoveRow.Name = "RemoveRow";
            this.RemoveRow.Size = new System.Drawing.Size(12, 12);
            this.RemoveRow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RemoveRow.TabIndex = 5;
            this.RemoveRow.TabStop = false;
            this.RemoveRow.Click += new System.EventHandler(this.DoRemoveRow);
            // 
            // RemoveColumn
            // 
            this.RemoveColumn.Image = global::Launcher.Properties.Resources.Remove;
            this.RemoveColumn.Location = new System.Drawing.Point(106, 27);
            this.RemoveColumn.Name = "RemoveColumn";
            this.RemoveColumn.Size = new System.Drawing.Size(12, 12);
            this.RemoveColumn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RemoveColumn.TabIndex = 7;
            this.RemoveColumn.TabStop = false;
            this.RemoveColumn.Click += new System.EventHandler(this.DoRemoveColumn);
            // 
            // AddColumn
            // 
            this.AddColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddColumn.Image = global::Launcher.Properties.Resources.Add;
            this.AddColumn.Location = new System.Drawing.Point(124, 27);
            this.AddColumn.Name = "AddColumn";
            this.AddColumn.Size = new System.Drawing.Size(12, 12);
            this.AddColumn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AddColumn.TabIndex = 6;
            this.AddColumn.TabStop = false;
            this.AddColumn.Click += new System.EventHandler(this.DoAddColumn);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(148, 114);
            this.Controls.Add(this.RemoveColumn);
            this.Controls.Add(this.AddColumn);
            this.Controls.Add(this.RemoveRow);
            this.Controls.Add(this.AddRow);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.RightClickMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AddRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RemoveRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RemoveColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AddColumn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip RightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RearrangeMenu;
        private System.Windows.Forms.ToolStripMenuItem gridSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddRowMenu;
        private System.Windows.Forms.ToolStripMenuItem AddColumnMenu;
        private System.Windows.Forms.ToolStripMenuItem removeEmptyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveRowMenu;
        private System.Windows.Forms.ToolStripMenuItem RemoveColumnMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
        private System.Windows.Forms.PictureBox AddRow;
        private System.Windows.Forms.PictureBox RemoveRow;
        private System.Windows.Forms.PictureBox RemoveColumn;
        private System.Windows.Forms.PictureBox AddColumn;
    }
}

