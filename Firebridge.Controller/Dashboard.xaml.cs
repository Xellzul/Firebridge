using Firebridge.Common.Models;
using Firebridge.Controller.Models;
using Firebridge.Controller.Common.Agents;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Firebridge.Controller;

public partial class Dashboard : ContentPage, IServiceCallback
{
    private readonly ILogger<Dashboard> logger;
    private readonly IDiscoveryClient discoveryClient;
    private readonly IControllerContext context;
    int count = 0;

    private ConcurrentDictionary<IServiceConnection, ServiceMiniView> views = new ConcurrentDictionary<IServiceConnection, ServiceMiniView>();

    public Dashboard(ILogger<Dashboard> logger, IDiscoveryClient discoveryClient, IControllerContext context)
    {
        this.logger = logger;
        this.discoveryClient = discoveryClient;
        this.context = context;

        InitializeComponent();

        for (int i = 0; i < 10; i++)
        {
            ServicesGrid.AddRowDefinition(new RowDefinition(90));
            ServicesGrid.AddColumnDefinition(new ColumnDefinition(160));
        }

        discoveryClient.StartDiscovering();

        //var timer = Application.Current.Dispatcher.CreateTimer();
        //timer.Interval = TimeSpan.FromSeconds(10);
        //timer.Tick += (s, e) => DoSomething();
        //timer.Start();
    }

    public void Connected(IServiceConnection serviceConnection)
    {
        var referenced = typeof(Class1TODODELETE).Assembly.GetReferencedAssemblies();

        var ams = new List<byte[]>();

        foreach (var referencedAssembly in referenced)
        {
            var a = AppDomain.CurrentDomain.GetAssemblies().
                SingleOrDefault(assembly => assembly.GetName().Name == referencedAssembly.Name)?.Location;

            if (a == null)
                continue;

            var dt = File.ReadAllBytes(a);
            ams.Add(dt);
        }

        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\System.Windows.Forms.dll"));
        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\System.Drawing.Common.dll"));
        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\System.Drawing.Primitives.dll"));
        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\System.Windows.Forms.Primitives.dll"));
        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\Accessibility.dll"));
        ams.Add(File.ReadAllBytes(@"C:\Users\xellz\source\repos\Firebridge2\bin\Controller\Debug\Microsoft.Win32.SystemEvents.dll"));

        ams.Add(File.ReadAllBytes(typeof(Class1TODODELETE).Assembly.Location));

        var sum = ams.Sum(x => x.Count());

        // Class1TODODELETE
        throw new Exception("TODO");

        serviceConnection.Send(new StartProgramModel() { AgentGuid=Guid.Parse("TODO"), SessionId = StartProgramModel.ActiveSessionId, Type = typeof(Class1TODODELETE).ToString() + ", " + typeof(Class1TODODELETE).Assembly.FullName, Assemblies = ams });

        this.Dispatcher.Dispatch(() =>
        {
            //Window secondWindow = new Window(new AppShell());
            //Application.Current.OpenWindow(secondWindow);

            views.TryGetValue(serviceConnection, out var view);

            view.BackgroundColor = Color.FromInt(new Random().Next());
            //view.Background = SolidColorBrush.DeepPink;
        });
    }

    public void Connecting(IServiceConnection serviceConnection)
    {
        this.Dispatcher.Dispatch(() =>
        {
            var view = new ServiceMiniView();
            var x = count % 10;
            var y = count / 10;

            views.TryAdd(serviceConnection, view);

            ServicesGrid.Add(view, x, y);

            count++;
        });

    }

    public void Disconnected(IServiceConnection serviceConnection)
    {
        this.Dispatcher.Dispatch(() =>
        {
            views.TryRemove(serviceConnection, out var src);

            ServicesGrid.Remove(src);
           // ServicesGrid.Clear();

            count--;
        });
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ServicePage));

        logger.LogCritical("HELLO");
        count++;
    }
}

