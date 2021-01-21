using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FirebridgeClient.Controls;
using FirebridgeShared.Models;
using FirebridgeShared.Networking;

namespace FirebridgeClient.Views
{
    public partial class ZombieDetailView : Form
    {
        private ZombieView _parent;
        public ZombieDetailView(ZombieView parent)
        {
            InitializeComponent();
            _parent = parent;

            zombieActionsBar1.zombieViews = new List<ZombieView> { _parent };

            this.BackColor = ColorTranslator.FromHtml("#2c2c54");
        }

        public void SetImage(Image image)
        {
            _image.Image = image;
        }

        private void ZombieDetailView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void _image_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (!cb_control.Checked)
                return;
            _parent.SendPacket(new Packet()
            {
                Id = 8,
                Data = new ClickModel()
                {
                    LeftMouse = (e.Button == MouseButtons.Left),
                    X = e.Location.X / (double)_image.Width,
                    Y = e.Location.Y / (double)_image.Height
                }
            });
            */
        }

        private void _image_MouseClick(object sender, MouseEventArgs e)
        { /*
            if (!cb_control.Checked)
                return;
            _parent.SendPacket(new Packet()
            {
                Id = 9,
                Data = new ClickModel()
                {
                    LeftMouse = (e.Button == MouseButtons.Left),
                    X = e.Location.X / (double)_image.Width,
                    Y = e.Location.Y / (double)_image.Height
                }
            });
            */
        }
    }
}
