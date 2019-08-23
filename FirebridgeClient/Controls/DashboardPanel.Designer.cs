namespace FirebridgeClient.Controls
{
    partial class DashboardPanel
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
            this._buttonSelectAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonSelectAll
            // 
            this._buttonSelectAll.Location = new System.Drawing.Point(14, 20);
            this._buttonSelectAll.Name = "_buttonSelectAll";
            this._buttonSelectAll.Size = new System.Drawing.Size(75, 23);
            this._buttonSelectAll.TabIndex = 0;
            this._buttonSelectAll.Text = "button1";
            this._buttonSelectAll.UseVisualStyleBackColor = true;
            this._buttonSelectAll.Click += new System.EventHandler(this._buttonSelectAll_Click);
            // 
            // DashboardPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._buttonSelectAll);
            this.Name = "DashboardPanel";
            this.Size = new System.Drawing.Size(340, 58);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonSelectAll;
    }
}
