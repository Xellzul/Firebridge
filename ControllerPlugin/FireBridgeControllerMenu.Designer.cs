
namespace ControllerPlugin
{
    partial class FireBridgeControllerMenu
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainView = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unlockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainView
            // 
            this.mainView.AllowDrop = true;
            this.mainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mainView.Location = new System.Drawing.Point(12, 27);
            this.mainView.Name = "mainView";
            this.mainView.Size = new System.Drawing.Size(1395, 767);
            this.mainView.TabIndex = 1;
            this.mainView.DragOver += new System.Windows.Forms.DragEventHandler(this.mainView_DragOver);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1419, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsMenu
            // 
            this.settingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenshotSettingsMenuItem,
            this.updateMenuItem,
            this.unlockMenuItem});
            this.settingsMenu.Name = "settingsMenu";
            this.settingsMenu.Size = new System.Drawing.Size(61, 20);
            this.settingsMenu.Text = "Settings";
            // 
            // screenshotSettingsMenuItem
            // 
            this.screenshotSettingsMenuItem.Name = "screenshotSettingsMenuItem";
            this.screenshotSettingsMenuItem.Size = new System.Drawing.Size(182, 22);
            this.screenshotSettingsMenuItem.Text = "Screenshots Settings";
            // 
            // updateMenuItem
            // 
            this.updateMenuItem.Name = "updateMenuItem";
            this.updateMenuItem.Size = new System.Drawing.Size(182, 22);
            this.updateMenuItem.Text = "updateMenuItem";
            // 
            // unlockMenuItem
            // 
            this.unlockMenuItem.Name = "unlockMenuItem";
            this.unlockMenuItem.Size = new System.Drawing.Size(182, 22);
            this.unlockMenuItem.Text = "Unlock PC";
            this.unlockMenuItem.Click += UnlockMenuItem_Click;
            // 
            // FireBridgeControllerMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1419, 806);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.mainView);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FireBridgeControllerMenu";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel mainView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem screenshotSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unlockMenuItem;
    }
}

