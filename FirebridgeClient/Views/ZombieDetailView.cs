using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FirebridgeClient.Controls;

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
    }
}
