using System;
using System.Drawing;

namespace FireBridgeController
{
    public class ImageRecievedEventArgs : EventArgs
    {
        public Image Image { get; set; }
    }
}