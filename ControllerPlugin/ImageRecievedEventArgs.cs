using System;
using System.Drawing;

namespace ControllerPlugin
{
    public class ImageRecievedEventArgs : EventArgs
    {
        public Image Image { get; set; }
    }
}