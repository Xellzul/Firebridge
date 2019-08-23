using System;
using System.Collections.Generic;
using System.Text;

namespace FirebridgeShared.Models
{
    [Serializable]
    public class ScreenshotRequestModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
