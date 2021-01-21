namespace FirebridgeClient.Controls
{
    partial class ZombieView
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
            this.components = new System.ComponentModel.Container();
            this._image = new System.Windows.Forms.PictureBox();
            this._ip = new System.Windows.Forms.Label();
            this._buttonDetail = new System.Windows.Forms.Button();
            this._rev = new System.Windows.Forms.Label();
            this._machineName = new System.Windows.Forms.Label();
            this._screenshotTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._image)).BeginInit();
            this.SuspendLayout();
            // 
            // _image
            // 
            this._image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._image.Location = new System.Drawing.Point(3, 3);
            this._image.Name = "_image";
            this._image.Size = new System.Drawing.Size(276, 166);
            this._image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._image.TabIndex = 0;
            this._image.TabStop = false;
            this._image.Click += new System.EventHandler(this._image_Click);
            // 
            // _ip
            // 
            this._ip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._ip.AutoSize = true;
            this._ip.Location = new System.Drawing.Point(3, 176);
            this._ip.Name = "_ip";
            this._ip.Size = new System.Drawing.Size(27, 13);
            this._ip.TabIndex = 1;
            this._ip.Text = "<ip>";
            // 
            // _buttonDetail
            // 
            this._buttonDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonDetail.Location = new System.Drawing.Point(194, 175);
            this._buttonDetail.Name = "_buttonDetail";
            this._buttonDetail.Size = new System.Drawing.Size(85, 38);
            this._buttonDetail.TabIndex = 4;
            this._buttonDetail.Text = "Detail";
            this._buttonDetail.UseVisualStyleBackColor = true;
            this._buttonDetail.Click += new System.EventHandler(this._buttonDetail_Click);
            // 
            // _rev
            // 
            this._rev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rev.AutoSize = true;
            this._rev.Location = new System.Drawing.Point(3, 189);
            this._rev.Name = "_rev";
            this._rev.Size = new System.Drawing.Size(34, 13);
            this._rev.TabIndex = 5;
            this._rev.Text = "<rev>";
            // 
            // _machineName
            // 
            this._machineName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._machineName.AutoSize = true;
            this._machineName.Location = new System.Drawing.Point(3, 200);
            this._machineName.Name = "_machineName";
            this._machineName.Size = new System.Drawing.Size(71, 13);
            this._machineName.TabIndex = 6;
            this._machineName.Text = "<machname>";
            // 
            // _screenshotTimer
            // 
            this._screenshotTimer.Enabled = true;
            this._screenshotTimer.Interval = 1000;
            this._screenshotTimer.Tick += new System.EventHandler(this._screenshotTimer_Tick);
            // 
            // ZombieView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._machineName);
            this.Controls.Add(this._rev);
            this.Controls.Add(this._buttonDetail);
            this.Controls.Add(this._ip);
            this.Controls.Add(this._image);
            this.Name = "ZombieView";
            this.Size = new System.Drawing.Size(282, 216);
            this.Click += new System.EventHandler(this._image_Click);
            ((System.ComponentModel.ISupportInitialize)(this._image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _image;
        private System.Windows.Forms.Label _ip;
        private System.Windows.Forms.Button _buttonDetail;
        private System.Windows.Forms.Label _rev;
        private System.Windows.Forms.Label _machineName;
        public System.Windows.Forms.Timer _screenshotTimer;
    }
}
