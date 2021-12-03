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
    public partial class DetailView : Form
    {
        public DetailView()
        {
            InitializeComponent();
        }

        private void SpyCam_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void p_screenshot_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
