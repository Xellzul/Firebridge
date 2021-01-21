using System;
using System.Windows.Forms;

namespace FireBridgeAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            Guid AgentID;
            if (args.Length > 0)
            {
                AgentID = Guid.Parse(args[0]);
                new Agent(AgentID).Start();
            }
            else
                new Agent().Start();
        }
    }
}
