namespace FirebridgeClient.Views
{
    partial class ZombieDetailView
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
            this._image = new System.Windows.Forms.PictureBox();
            this.zombieActionsBar1 = new FirebridgeClient.Controls.ZombieActionsBar();
            ((System.ComponentModel.ISupportInitialize)(this._image)).BeginInit();
            this.SuspendLayout();
            // 
            // _image
            // 
            this._image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._image.Location = new System.Drawing.Point(13, 14);
            this._image.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._image.Name = "_image";
            this._image.Size = new System.Drawing.Size(599, 506);
            this._image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._image.TabIndex = 4;
            this._image.TabStop = false;
            // 
            // zombieActionsBar1
            // 
            this.zombieActionsBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zombieActionsBar1.BackColor = System.Drawing.Color.Transparent;
            this.zombieActionsBar1.Location = new System.Drawing.Point(619, 14);
            this.zombieActionsBar1.Name = "zombieActionsBar1";
            this.zombieActionsBar1.Size = new System.Drawing.Size(165, 506);
            this.zombieActionsBar1.TabIndex = 5;
            this.zombieActionsBar1.zombieViews = null;
            // 
            // ZombieDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 532);
            this.Controls.Add(this.zombieActionsBar1);
            this.Controls.Add(this._image);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ZombieDetailView";
            this.Text = "ZombieDetailView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZombieDetailView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this._image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox _image;
        private Controls.ZombieActionsBar zombieActionsBar1;
    }
}