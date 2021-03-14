
namespace ControllerPlugin
{
    partial class ScreenshotSettings
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
            this.cb_enabled = new System.Windows.Forms.CheckBox();
            this.cb_quality = new System.Windows.Forms.ComboBox();
            this.cb_resolution = new System.Windows.Forms.ComboBox();
            this.cb_speed = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cb_enabled
            // 
            this.cb_enabled.AutoSize = true;
            this.cb_enabled.Checked = true;
            this.cb_enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_enabled.Location = new System.Drawing.Point(159, 51);
            this.cb_enabled.Name = "cb_enabled";
            this.cb_enabled.Size = new System.Drawing.Size(68, 19);
            this.cb_enabled.TabIndex = 0;
            this.cb_enabled.Text = "Enabled";
            this.cb_enabled.UseVisualStyleBackColor = true;
            this.cb_enabled.CheckedChanged += new System.EventHandler(this.cb_enabled_CheckedChanged);
            // 
            // cb_quality
            // 
            this.cb_quality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_quality.FormattingEnabled = true;
            this.cb_quality.Location = new System.Drawing.Point(138, 76);
            this.cb_quality.Name = "cb_quality";
            this.cb_quality.Size = new System.Drawing.Size(121, 23);
            this.cb_quality.TabIndex = 1;
            // 
            // cb_resolution
            // 
            this.cb_resolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_resolution.FormattingEnabled = true;
            this.cb_resolution.Location = new System.Drawing.Point(138, 105);
            this.cb_resolution.Name = "cb_resolution";
            this.cb_resolution.Size = new System.Drawing.Size(121, 23);
            this.cb_resolution.TabIndex = 2;
            // 
            // cb_speed
            // 
            this.cb_speed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_speed.FormattingEnabled = true;
            this.cb_speed.Location = new System.Drawing.Point(138, 134);
            this.cb_speed.Name = "cb_speed";
            this.cb_speed.Size = new System.Drawing.Size(121, 23);
            this.cb_speed.TabIndex = 3;
            // 
            // ScreenshotSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 233);
            this.Controls.Add(this.cb_speed);
            this.Controls.Add(this.cb_resolution);
            this.Controls.Add(this.cb_quality);
            this.Controls.Add(this.cb_enabled);
            this.Name = "ScreenshotSettings";
            this.Text = "ScreenshotSettings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScreenshotSettings_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_enabled;
        private System.Windows.Forms.ComboBox cb_quality;
        private System.Windows.Forms.ComboBox cb_resolution;
        private System.Windows.Forms.ComboBox cb_speed;
    }
}