using System;
using System.Threading;
using System.Windows.Forms;

namespace FireBridgeAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            new Agent().Start();
        }
    }
}
