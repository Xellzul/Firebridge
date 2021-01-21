using FireBridgeCore.Kernel.UserPrograms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBridgeController
{
    public partial class ScreenshotSettings : Form
    {
        public OverridePorgramSettings ops;
        public ScreenshotSettings(OverridePorgramSettings OverridePorgramSettings)
        {
            ops = OverridePorgramSettings;
            InitializeComponent(); 
        }

        private void cb_speed_SelectedValueChanged(object sender, EventArgs e)
        {
            switch(cb_speed.SelectedItem.ToString())
            {
                case "60hz":
                    ops.WaitTime = (int)(1000f / 60);
                    break;
                case "45hz":
                    ops.WaitTime = (int)(1000f / 45);
                    break;
                case "30hz":
                    ops.WaitTime = (int)(1000f / 30);
                    break;
                case "15hz":
                    ops.WaitTime = (int)(1000f / 15);
                    break;
                case "10hz":
                    ops.WaitTime = (int)(1000f / 10);
                    break;
                case "5hz":
                    ops.WaitTime = (int)(1000f / 5);
                    break;
                case "3hz":
                    ops.WaitTime = (int)(1000f / 3);
                    break;
                case "1hz":
                    ops.WaitTime = (int)(1000f / 1);
                    break;
                case "2s":
                    ops.WaitTime = (int)(1000f * 2);
                    break;
                case "4s":
                    ops.WaitTime = (int)(1000f * 4);
                    break;
                case "8s":
                    ops.WaitTime = (int)(1000f * 8);
                    break;
                default:
                    break;
            }
            OnSettingsChanged(e);
        }

        private void cb_resolution_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (cb_resolution.SelectedItem.ToString())
            {
                case "72p":
                    ops.Width = 128;
                    ops.Heigth = 72;
                    break;
                case "144p":
                    ops.Width = 256;
                    ops.Heigth = 144;
                    break;
                case "240p":
                    ops.Width = 426;
                    ops.Heigth = 240;
                    break;
                case "360p":
                    ops.Width = 640;
                    ops.Heigth = 360;
                    break;
                case "480p":
                    ops.Width = 720;
                    ops.Heigth = 480;
                    break;
                case "720p":
                    ops.Width = 1280;
                    ops.Heigth = 720;
                    break;
                case "1080p":
                    ops.Width = 1920;
                    ops.Heigth = 1080;
                    break;
                case "1440p":
                    ops.Width = 2560;
                    ops.Heigth = 1440;
                    break;
                case "2160p":
                    ops.Width = 3840;
                    ops.Heigth = 2160;
                    break;
                case "4320p":
                    ops.Width = 7680;
                    ops.Heigth = 4320;
                    break;
                default:
                    break;
            }
            OnSettingsChanged(e);
        }

        private void cb_quality_SelectedValueChanged(object sender, EventArgs e)
        {
            Int32.TryParse(cb_quality.SelectedItem.ToString(), out int quality);
            ops.Quality = quality;
            OnSettingsChanged(e);
        }

        private void cb_enabled_CheckedChanged(object sender, EventArgs e)
        {
            ops.TakeScreenShot = cb_enabled.Checked;
            OnSettingsChanged(e);
        }

        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (SettingsChanged == null || e == null)
                return;

            SettingsChanged.Invoke(this, e);
        }
        public event EventHandler SettingsChanged;

        private void ScreenshotSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
