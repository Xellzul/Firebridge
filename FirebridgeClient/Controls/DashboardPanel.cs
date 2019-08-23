using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient.Controls
{
    public partial class DashboardPanel : UserControl
    {
        public DashboardPanel()
        {
            InitializeComponent();
            this.BackColor = ColorTranslator.FromHtml("#ffda79");
        }

        private void _buttonSelectAll_Click(object sender, EventArgs e)
        {

        }
    }
}
