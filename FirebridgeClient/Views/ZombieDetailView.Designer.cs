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
            this.cb_control = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._image)).BeginInit();
            this.SuspendLayout();
            // 
            // _image
            // 
            this._image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._image.Location = new System.Drawing.Point(9, 9);
            this._image.Name = "_image";
            this._image.Size = new System.Drawing.Size(945, 577);
            this._image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._image.TabIndex = 4;
            this._image.TabStop = false;
            this._image.MouseClick += new System.Windows.Forms.MouseEventHandler(this._image_MouseClick);
            this._image.MouseMove += new System.Windows.Forms.MouseEventHandler(this._image_MouseMove);
            // 
            // zombieActionsBar1
            // 
            this.zombieActionsBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zombieActionsBar1.BackColor = System.Drawing.Color.Transparent;
            this.zombieActionsBar1.Location = new System.Drawing.Point(959, 41);
            this.zombieActionsBar1.Margin = new System.Windows.Forms.Padding(2);
            this.zombieActionsBar1.Name = "zombieActionsBar1";
            this.zombieActionsBar1.Size = new System.Drawing.Size(110, 545);
            this.zombieActionsBar1.TabIndex = 5;
            this.zombieActionsBar1.zombieViews = null;
            // 
            // cb_control
            // 
            this.cb_control.AutoSize = true;
            this.cb_control.Location = new System.Drawing.Point(974, 19);
            this.cb_control.Name = "cb_control";
            this.cb_control.Size = new System.Drawing.Size(59, 17);
            this.cb_control.TabIndex = 6;
            this.cb_control.Text = "Control";
            this.cb_control.UseVisualStyleBackColor = true;
            // 
            // ZombieDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 594);
            this.Controls.Add(this.cb_control);
            this.Controls.Add(this.zombieActionsBar1);
            this.Controls.Add(this._image);
            this.Name = "ZombieDetailView";
            this.Text = "ZombieDetailView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZombieDetailView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this._image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox _image;
        private Controls.ZombieActionsBar zombieActionsBar1;
        private System.Windows.Forms.CheckBox cb_control;
    }
}