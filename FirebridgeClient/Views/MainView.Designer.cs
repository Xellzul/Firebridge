namespace FirebridgeClient
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this._devices = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // _devices
            // 
            this._devices.AutoSize = true;
            this._devices.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._devices.Location = new System.Drawing.Point(30, 27);
            this._devices.Name = "_devices";
            this._devices.Size = new System.Drawing.Size(108, 19);
            this._devices.TabIndex = 2;
            this._devices.Text = "Devices found:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.sendUpdateToolStripMenuItem,
            this.lockAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // sendUpdateToolStripMenuItem
            // 
            this.sendUpdateToolStripMenuItem.Name = "sendUpdateToolStripMenuItem";
            this.sendUpdateToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sendUpdateToolStripMenuItem.Text = "Send update ";
            this.sendUpdateToolStripMenuItem.Click += new System.EventHandler(this.SendUpdateToolStripMenuItem_Click);
            // 
            // lockAllToolStripMenuItem
            // 
            this.lockAllToolStripMenuItem.Name = "lockAllToolStripMenuItem";
            this.lockAllToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.lockAllToolStripMenuItem.Text = "Lock all";
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1347, 561);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this._devices);
            this.Name = "MainView";
            this.Text = "Firebridge™ Dashboard";
            this.Resize += new System.EventHandler(this.MainView_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label _devices;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockAllToolStripMenuItem;
    }
}

