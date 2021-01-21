using System.Windows.Forms;

namespace FireBridgeController
{
    partial class OverViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imagePanel = new FireBridgeController.DBPanel();
            this.l_connecting = new System.Windows.Forms.Label();
            this.toolsPanel = new System.Windows.Forms.Panel();
            this.b_detail = new System.Windows.Forms.Button();
            this.l_ip = new System.Windows.Forms.Label();
            this.imagePanel.SuspendLayout();
            this.toolsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imagePanel.Controls.Add(this.l_connecting);
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(480, 270);
            this.imagePanel.TabIndex = 0;
            // 
            // l_connecting
            // 
            this.l_connecting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.l_connecting.AutoSize = true;
            this.l_connecting.Font = new System.Drawing.Font("Verdana", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.l_connecting.Location = new System.Drawing.Point(31, 96);
            this.l_connecting.Name = "l_connecting";
            this.l_connecting.Size = new System.Drawing.Size(409, 80);
            this.l_connecting.TabIndex = 0;
            this.l_connecting.Text = "Connecting";
            // 
            // toolsPanel
            // 
            this.toolsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolsPanel.Controls.Add(this.b_detail);
            this.toolsPanel.Controls.Add(this.l_ip);
            this.toolsPanel.Location = new System.Drawing.Point(0, 269);
            this.toolsPanel.Name = "toolsPanel";
            this.toolsPanel.Size = new System.Drawing.Size(440, 48);
            this.toolsPanel.TabIndex = 0;
            this.toolsPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.toolsPanel_MouseClick);
            // 
            // b_detail
            // 
            this.b_detail.Location = new System.Drawing.Point(349, 0);
            this.b_detail.Name = "b_detail";
            this.b_detail.Size = new System.Drawing.Size(91, 48);
            this.b_detail.TabIndex = 1;
            this.b_detail.Text = "Detail";
            this.b_detail.UseVisualStyleBackColor = true;
            this.b_detail.Click += new System.EventHandler(this.b_detail_Click);
            // 
            // l_ip
            // 
            this.l_ip.AutoSize = true;
            this.l_ip.Location = new System.Drawing.Point(0, 4);
            this.l_ip.Name = "l_ip";
            this.l_ip.Size = new System.Drawing.Size(38, 15);
            this.l_ip.TabIndex = 0;
            this.l_ip.Text = "label1";
            // 
            // OverViewControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolsPanel);
            this.Controls.Add(this.imagePanel);
            this.Name = "OverViewControl";
            this.Size = new System.Drawing.Size(480, 317);
            this.imagePanel.ResumeLayout(false);
            this.imagePanel.PerformLayout();
            this.toolsPanel.ResumeLayout(false);
            this.toolsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DBPanel imagePanel;
        private Panel toolsPanel;
        private Label l_ip;
        private Label l_connecting;
        private Button b_detail;
    }
}
