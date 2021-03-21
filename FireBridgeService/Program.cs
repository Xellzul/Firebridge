using System;
using System.ServiceProcess;

namespace FireBridgeService
{
    class Program
    {
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                string ServiceName = "FireBridge";
                var status = ServiceInstaller.GetServiceStatus(ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());

                if(status != ServiceState.Running && status != ServiceState.NotFound)
                {
                    Console.WriteLine("Stopping old serice"); 
                    ServiceInstaller.StopService(ServiceName);
                    status = ServiceInstaller.GetServiceStatus(ServiceName);
                    Console.WriteLine("Service status is: " + status.ToString());
                }

                if(ServiceInstaller.ServiceIsInstalled(ServiceName))
                {
                    Console.WriteLine("Uninstalling old serice");
                    ServiceInstaller.Uninstall(ServiceName); 
                    status = ServiceInstaller.GetServiceStatus(ServiceName);
                    Console.WriteLine("Service status is: " + status.ToString());
                }
                

                Console.WriteLine("Installing Service");

                ServiceInstaller.InstallAndStart(ServiceName, ServiceName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

                status = ServiceInstaller.GetServiceStatus(ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());

                Console.WriteLine("Installed, press any key to continue");
                Console.ReadKey(true);
                return;
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FireBridgeService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
