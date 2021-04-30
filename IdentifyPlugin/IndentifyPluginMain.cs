using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentifyPlugin
{
    [FireBridgePlugin]
    class IndentifyPluginMain : FireBridgePlugin
    {
        public override int Order => 21000;

        public IndentifyPluginMain() : base()
        {
            PossibleActions.Add(new KeyValuePair<string, EventHandler>("Identify", new EventHandler(Identify)));
        }

        public override void Start()
        {
        }

        private void Identify(object sender, EventArgs e)
        {
            foreach (var sc in ConnectionManger.Instance.GetSelectedServices())
                sc.StartProgram(typeof(IdentifyProcess), IIntegrityLevel.Medium, UInt32.MaxValue, AssemblyData, null, null);
        }
    }
}
