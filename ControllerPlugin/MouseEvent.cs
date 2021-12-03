using System;

namespace ControllerPlugin
{
    [Serializable]
    public class MouseEvent
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
    }
}