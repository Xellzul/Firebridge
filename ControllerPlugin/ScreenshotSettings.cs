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

namespace ControllerPlugin
{
    public partial class ScreenshotSettings : Form
    {
        public OverridePorgramSettings ops;
        private Dictionary<string, ValueTuple<int, int>> resolutions = new Dictionary<string, ValueTuple<int, int>>() {
            { "72p", (128, 72) },
            {"144p", (256, 144) },
            {"240p", (426, 240) },
            {"360p", (640, 360) },
            {"480p", (720, 480) },
            {"720p", (1280, 720) },
            {"1080p", (1920, 1080) },
            {"1440p", (2560, 1440) },
            {"2160p", (3840, 2160) },
            {"4320p", (7680, 4320) } 
        };

        private Dictionary<string, int> qualities = new Dictionary<string, int>() {
            {"100", 100},
            {"90", 90 },
            {"80", 80 },
            {"70", 70 },
            {"60", 60 },
            {"50", 50 },
            {"40", 40 },
            {"30", 30 },
            {"20", 20 },
            {"10", 10 },
            {"0", 0 }
        };

        private Dictionary<string, int> speeds = new Dictionary<string, int>() {
            {"60hz", (int)(1000f / 60)},
            {"45hz", (int)(1000f / 45) },
            {"30hz", (int)(1000f / 30) },
            {"15hz", (int)(1000f / 45) },
            {"10hz", (int)(1000f / 10) },
            {"5hz", (int)(1000f / 5) },
            {"3hz", (int)(1000f / 3) },
            {"1hz", (int)(1000f) },
            {"2s", (int)(1000f * 2) },
            {"4s", (int)(1000f * 4) },
            {"8s", (int)(1000f * 8) }
        };


        public ScreenshotSettings(OverridePorgramSettings OverridePorgramSettings)
        {
            ops = OverridePorgramSettings;
            InitializeComponent();

            this.cb_quality.Items.AddRange(qualities.Keys.ToArray());
            this.cb_resolution.Items.AddRange(resolutions.Keys.ToArray());
            this.cb_speed.Items.AddRange(speeds.Keys.ToArray());

            cb_quality.SelectedIndex = cb_quality.FindStringExact(qualities.Where(x => x.Value == ops.Quality).First().Key);
            cb_resolution.SelectedIndex = cb_resolution.FindStringExact(resolutions.Where(x => x.Value.Item1 == ops.Width && x.Value.Item2 == ops.Heigth).First().Key);
            cb_speed.SelectedIndex = cb_speed.FindStringExact(speeds.Where(x=> x.Value == ops.WaitTime).First().Key);

            this.cb_quality.SelectedValueChanged += new System.EventHandler(this.cb_quality_SelectedValueChanged);
            this.cb_resolution.SelectedValueChanged += new System.EventHandler(this.cb_resolution_SelectedValueChanged);
            this.cb_speed.SelectedValueChanged += new System.EventHandler(this.cb_speed_SelectedValueChanged);
        }

        private void cb_speed_SelectedValueChanged(object sender, EventArgs e)
        {
            ops.WaitTime = speeds[cb_speed.SelectedItem.ToString()];
            OnSettingsChanged(e);
        }

        private void cb_resolution_SelectedValueChanged(object sender, EventArgs e)
        {
            var values = resolutions[cb_resolution.SelectedItem.ToString()];
            ops.Width = values.Item1;
            ops.Heigth = values.Item2;
            OnSettingsChanged(e);
        }

        private void cb_quality_SelectedValueChanged(object sender, EventArgs e)
        {
            ops.Quality = qualities[cb_quality.SelectedItem.ToString()];
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
