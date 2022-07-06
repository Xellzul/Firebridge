
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
            this.p_screenshot = new System.Windows.Forms.Panel();
            this.cb_keyboard = new System.Windows.Forms.CheckBox();
            this.cb_mouse = new System.Windows.Forms.CheckBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // p_screenshot
            // 
            this.p_screenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.p_screenshot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.p_screenshot.Location = new System.Drawing.Point(384, 12);
            this.p_screenshot.Name = "p_screenshot";
            this.p_screenshot.Size = new System.Drawing.Size(994, 715);
            this.p_screenshot.TabIndex = 0;
            this.p_screenshot.MouseClick += new System.Windows.Forms.MouseEventHandler(this.p_screenshot_MouseClick);
            // 
            // cb_keyboard
            // 
            this.cb_keyboard.AutoSize = true;
            this.cb_keyboard.Location = new System.Drawing.Point(10, 12);
            this.cb_keyboard.Name = "cb_keyboard";
            this.cb_keyboard.Size = new System.Drawing.Size(76, 19);
            this.cb_keyboard.TabIndex = 1;
            this.cb_keyboard.Text = "Keyboard";
            this.cb_keyboard.UseVisualStyleBackColor = true;
            // 
            // cb_mouse
            // 
            this.cb_mouse.AutoSize = true;
            this.cb_mouse.Location = new System.Drawing.Point(10, 37);
            this.cb_mouse.Name = "cb_mouse";
            this.cb_mouse.Size = new System.Drawing.Size(62, 19);
            this.cb_mouse.TabIndex = 2;
            this.cb_mouse.Text = "Mouse";
            this.cb_mouse.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(10, 62);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(368, 435);
            this.propertyGrid1.TabIndex = 3;
            // 
            // DetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1390, 784);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.cb_mouse);
            this.Controls.Add(this.cb_keyboard);
            this.Controls.Add(this.p_screenshot);
            this.Name = "DetailView";
            this.Text = "SpyCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpyCam_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel p_screenshot;
        public System.Windows.Forms.CheckBox cb_keyboard;
        public System.Windows.Forms.CheckBox cb_mouse;
        public System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}