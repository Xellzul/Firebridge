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
            this.button1 = new System.Windows.Forms.Button();
            this.todo_delete_this = new System.Windows.Forms.Button();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(170, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Detail";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // todo_delete_this
            // 
            this.todo_delete_this.Location = new System.Drawing.Point(138, 158);
            this.todo_delete_this.Name = "todo_delete_this";
            this.todo_delete_this.Size = new System.Drawing.Size(26, 23);
            this.todo_delete_this.TabIndex = 3;
            this.todo_delete_this.Text = "U";
            this.todo_delete_this.UseVisualStyleBackColor = true;
            this.todo_delete_this.Click += new System.EventHandler(this.Todo_delete_this_Click);
            // 
            // ZombieView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.todo_delete_this);
            this.Controls.Add(this.button1);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button todo_delete_this;
    }
}
