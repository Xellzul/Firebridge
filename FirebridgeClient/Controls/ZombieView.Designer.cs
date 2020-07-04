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
            this._image.Location = new System.Drawing.Point(4, 5);
            this._image.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._image.Name = "_image";
            this._image.Size = new System.Drawing.Size(363, 211);
            this._image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._image.TabIndex = 0;
            this._image.TabStop = false;
            this._image.Click += new System.EventHandler(this._image_Click);
            // 
            // _ip
            // 
            this._ip.AutoSize = true;
            this._ip.Location = new System.Drawing.Point(4, 221);
            this._ip.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._ip.Name = "_ip";
            this._ip.Size = new System.Drawing.Size(39, 20);
            this._ip.TabIndex = 1;
            this._ip.Text = "<ip>";
            // 
            // _buttonDetail
            // 
            this._buttonDetail.Location = new System.Drawing.Point(239, 221);
            this._buttonDetail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._buttonDetail.Name = "_buttonDetail";
            this._buttonDetail.Size = new System.Drawing.Size(128, 58);
            this._buttonDetail.TabIndex = 4;
            this._buttonDetail.Text = "Detail";
            this._buttonDetail.UseVisualStyleBackColor = true;
            this._buttonDetail.Click += new System.EventHandler(this._buttonDetail_Click);
            // 
            // _rev
            // 
            this._rev.AutoSize = true;
            this._rev.Location = new System.Drawing.Point(4, 241);
            this._rev.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._rev.Name = "_rev";
            this._rev.Size = new System.Drawing.Size(48, 20);
            this._rev.TabIndex = 5;
            this._rev.Text = "<rev>";
            // 
            // _machineName
            // 
            this._machineName.AutoSize = true;
            this._machineName.Location = new System.Drawing.Point(4, 258);
            this._machineName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._machineName.Name = "_machineName";
            this._machineName.Size = new System.Drawing.Size(106, 20);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._machineName);
            this.Controls.Add(this._rev);
            this.Controls.Add(this._buttonDetail);
            this.Controls.Add(this._ip);
            this.Controls.Add(this._image);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ZombieView";
            this.Size = new System.Drawing.Size(372, 284);
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
