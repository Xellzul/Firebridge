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
            this._image = new System.Windows.Forms.PictureBox();
            this._ip = new System.Windows.Forms.Label();
            this.todo_delete_this = new System.Windows.Forms.Button();
            this._buttonDetail = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._image)).BeginInit();
            this.SuspendLayout();
            // 
            // _image
            // 
            this._image.Location = new System.Drawing.Point(3, 3);
            this._image.Name = "_image";
            this._image.Size = new System.Drawing.Size(242, 137);
            this._image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._image.TabIndex = 0;
            this._image.TabStop = false;
            this._image.Click += new System.EventHandler(this._image_Click);
            // 
            // _ip
            // 
            this._ip.AutoSize = true;
            this._ip.Location = new System.Drawing.Point(3, 163);
            this._ip.Name = "_ip";
            this._ip.Size = new System.Drawing.Size(27, 13);
            this._ip.TabIndex = 1;
            this._ip.Text = "<ip>";
            // 
            // todo_delete_this
            // 
            this.todo_delete_this.Location = new System.Drawing.Point(128, 158);
            this.todo_delete_this.Name = "todo_delete_this";
            this.todo_delete_this.Size = new System.Drawing.Size(26, 23);
            this.todo_delete_this.TabIndex = 3;
            this.todo_delete_this.Text = "U";
            this.todo_delete_this.UseVisualStyleBackColor = true;
            this.todo_delete_this.Click += new System.EventHandler(this.Todo_delete_this_Click);
            // 
            // _buttonDetail
            // 
            this._buttonDetail.Location = new System.Drawing.Point(160, 158);
            this._buttonDetail.Name = "_buttonDetail";
            this._buttonDetail.Size = new System.Drawing.Size(75, 23);
            this._buttonDetail.TabIndex = 4;
            this._buttonDetail.Text = "Detail";
            this._buttonDetail.UseVisualStyleBackColor = true;
            this._buttonDetail.Click += new System.EventHandler(this._buttonDetail_Click);
            // 
            // ZombieView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._buttonDetail);
            this.Controls.Add(this.todo_delete_this);
            this.Controls.Add(this._ip);
            this.Controls.Add(this._image);
            this.Name = "ZombieView";
            this.Size = new System.Drawing.Size(248, 193);
            this.Click += new System.EventHandler(this._image_Click);
            ((System.ComponentModel.ISupportInitialize)(this._image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _image;
        private System.Windows.Forms.Label _ip;
        private System.Windows.Forms.Button todo_delete_this;
        private System.Windows.Forms.Button _buttonDetail;
    }
}
