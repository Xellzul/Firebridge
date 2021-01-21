using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel.UserPrograms
{
    [Serializable]
    public class OverridePorgramSettings
    {
        public int WaitTime { get; set; } = 1000;
        public int Quality { get; set; } = 70;
        public int Width { get; set; } = 720;
        public int Heigth { get; set; } = 480;
        public bool TakeScreenShot { get; set; } = true;
    }
}
