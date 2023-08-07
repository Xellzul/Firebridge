namespace Firebridge.Controller;

public class AppShell : Shell
{
	public AppShell()
    {
        this.FlyoutBehavior = FlyoutBehavior.Disabled;

        this.CurrentItem = new ShellContent()
        {
            ContentTemplate = new DataTemplate(typeof(Dashboard)),
            Route = "Dashboard"
        };

        Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
        Routing.RegisterRoute(nameof(ServicePage), typeof(ServicePage));
    }
}