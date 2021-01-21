using System.Drawing;

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
            this.pingTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._bRight = new System.Windows.Forms.Button();
            this._bLeft = new System.Windows.Forms.Button();
            this.tb_size = new System.Windows.Forms.TrackBar();
            this.zombieActionsBar1 = new FirebridgeClient.Controls.ZombieActionsBar();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_size)).BeginInit();
            this.SuspendLayout();
            // 
            // pingTimer
            // 
            this.pingTimer.Interval = 999;
            this.pingTimer.Tick += new System.EventHandler(this.pingTimer_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 26);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(11, 8);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1593, 734);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // _bRight
            // 
            this._bRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRight.Location = new System.Drawing.Point(1668, 8);
            this._bRight.Name = "_bRight";
            this._bRight.Size = new System.Drawing.Size(50, 42);
            this._bRight.TabIndex = 9;
            this._bRight.Text = ">";
            this._bRight.UseVisualStyleBackColor = true;
            this._bRight.Click += new System.EventHandler(this._bRight_Click);
            // 
            // _bLeft
            // 
            this._bLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bLeft.Location = new System.Drawing.Point(1608, 8);
            this._bLeft.Name = "_bLeft";
            this._bLeft.Size = new System.Drawing.Size(50, 42);
            this._bLeft.TabIndex = 8;
            this._bLeft.Text = "<";
            this._bLeft.UseVisualStyleBackColor = true;
            this._bLeft.Click += new System.EventHandler(this._bLeft_Click);
            // 
            // tb_size
            // 
            this.tb_size.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_size.Location = new System.Drawing.Point(1611, 56);
            this.tb_size.Maximum = 600;
            this.tb_size.Minimum = 25;
            this.tb_size.Name = "tb_size";
            this.tb_size.Size = new System.Drawing.Size(107, 45);
            this.tb_size.TabIndex = 10;
            this.tb_size.Value = 100;
            this.tb_size.ValueChanged += new System.EventHandler(this.tb_size_ValueChanged);
            // 
            // zombieActionsBar1
            // 
            this.zombieActionsBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zombieActionsBar1.BackColor = System.Drawing.Color.Transparent;
            this.zombieActionsBar1.Location = new System.Drawing.Point(1608, 111);
            this.zombieActionsBar1.Margin = new System.Windows.Forms.Padding(2);
            this.zombieActionsBar1.Name = "zombieActionsBar1";
            this.zombieActionsBar1.Size = new System.Drawing.Size(110, 631);
            this.zombieActionsBar1.TabIndex = 5;
            this.zombieActionsBar1.zombieViews = null;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.ClientSize = new System.Drawing.Size(1726, 750);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.tb_size);
            this.Controls.Add(this._bRight);
            this.Controls.Add(this._bLeft);
            this.Controls.Add(this.zombieActionsBar1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "MainView";
            this.Text = "Firebridge™ Dashboard";
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tb_size)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer pingTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Controls.ZombieActionsBar zombieActionsBar1;
        private System.Windows.Forms.Button _bRight;
        private System.Windows.Forms.Button _bLeft;
        private System.Windows.Forms.TrackBar tb_size;
    }
}

