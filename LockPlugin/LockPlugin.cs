using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockPlugin
{
    [FireBridgePlugin]
    public class LockPlugin : FireBridgePlugin
    {
        public override ICollection<KeyValuePair<string, Action<ServiceConnection[]>>> Register()
        {
            return new List<KeyValuePair<string, Action<ServiceConnection[]>>>() {
                new KeyValuePair<string, Action<ServiceConnection[]>>
                    ("Lock PC", new Action<ServiceConnection[]>(LockPC))
            };
        }

        private void LockPC(ServiceConnection[] obj)
        {
            foreach (var sc in obj)
            {
                var agent = sc.GetAgent(IIntegrityLevel.High, 0);
                if (agent == null)
                    continue;

                sc.StartProgram(agent, new LockProcess());
            }
        }
    }
}
