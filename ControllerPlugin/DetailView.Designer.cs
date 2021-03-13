
namespace ControllerPlugin
{
    partial class DetailView
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
            this.p_screenshot = new DBPanel();
            this.SuspendLayout();
            // 
            // p_screenshot
            // 
            this.p_screenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.p_screenshot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.p_screenshot.Location = new System.Drawing.Point(12, 12);
            this.p_screenshot.Name = "p_screenshot";
            this.p_screenshot.Size = new System.Drawing.Size(1366, 715);
            this.p_screenshot.TabIndex = 0;
            // 
            // DetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1390, 784);
            this.Controls.Add(this.p_screenshot);
            this.Name = "DetailView";
            this.Text = "SpyCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpyCam_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public DBPanel p_screenshot;
    }
}