using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace FireBridgeZombie
{
    class Program
    {
        static void Main()
        {
            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            Trace.Listeners.Add(new TextWriterTraceListener(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\zombie.txt"));
            Trace.AutoFlush = true;

            Trace.WriteLine("Firebridge Zombie started");
            Trace.WriteLine("Eelevated: " + IsElevated);
            new Zombie().Start();
        }
    }
}