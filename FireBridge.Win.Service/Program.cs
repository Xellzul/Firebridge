using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace FireBridge.Win.Service;

class Program
{
    static async Task Main()
    {
        if (Environment.UserInteractive)
        {
            string ServiceName = "FireBridge2";
            var status = ServiceInstaller.GetServiceStatus(ServiceName);
            Console.WriteLine("Service status is: " + status.ToString());

            if (status != ServiceState.Running && status != ServiceState.NotFound)
            {
                Console.WriteLine("Stopping old serice");
                ServiceInstaller.StopService(ServiceName);
                status = ServiceInstaller.GetServiceStatus(ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());
            }

            if (ServiceInstaller.ServiceIsInstalled(ServiceName))
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

        await FireBridge.Service.Program.Main();
    }
}
