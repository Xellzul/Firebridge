using Firebridge.Controller.Common;
using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.Collections.Concurrent;

namespace Firebridge.Controller;

public partial class Dashboard : ContentPage
{
    private readonly ILogger<Dashboard> logger;
    private readonly IDiscoveryClient discoveryClient;
    private readonly IControllerContext context;
    private readonly IMediator mediator;
    int count = 0;

    private ConcurrentDictionary<IServiceConnection, ServiceMiniView> views = new ConcurrentDictionary<IServiceConnection, ServiceMiniView>();

    public Dashboard(ILogger<Dashboard> logger, IDiscoveryClient discoveryClient, IControllerContext context, IMediator mediator)
    {
        this.logger = logger;
        this.discoveryClient = discoveryClient;
        this.context = context;
        this.mediator = mediator;

        InitializeComponent();

        for (int i = 0; i < 10; i++)
        {
            ServicesGrid.AddRowDefinition(new RowDefinition(90));
            ServicesGrid.AddColumnDefinition(new ColumnDefinition(160));
        }

        foreach (var globalAction in context.GetGlobalActions())
        {
            var button = new Button() { Text = globalAction };
            button.CommandParameter = globalAction;
            button.Clicked += async (s, e) => { await mediator.Publish(new ControllerGlobalActionNotification() { Action = ((Button)s).CommandParameter as string }); };
            GlobalActions.Add(button);
        }

        discoveryClient.StartDiscovering();

        //var timer = Application.Current.Dispatcher.CreateTimer();
        //timer.Interval = TimeSpan.FromSeconds(10);
        //timer.Tick += (s, e) => DoSomething();
        //timer.Start();
    }

    public void Connected(IServiceConnection serviceConnection)
    {
        this.Dispatcher.Dispatch(() =>
        {
            //Window secondWindow = new Window(new AppShell());
            //Application.Current.OpenWindow(secondWindow);

            views.TryGetValue(serviceConnection, out var view);

            view.BackgroundColor = Color.FromInt(new Random().Next());
            //view.Background = SolidColorBrush.DeepPink;
        });
    }

    private void Connecting(IServiceConnection serviceConnection)
    {
        this.Dispatcher.Dispatch(() =>
        {
            var view = serviceConnection.Scope.ServiceProvider.GetRequiredService<ServiceMiniView>();

            var x = count % 10;
            var y = count / 10;

            views.TryAdd(serviceConnection, view);

            ServicesGrid.Add(view, x, y);

            count++;
        });

    }

    private void Disconnected(IServiceConnection serviceConnection)
    {
        this.Dispatcher.Dispatch(() =>
        {
            views.TryRemove(serviceConnection, out var src);

            ServicesGrid.Remove(src);
           // ServicesGrid.Clear();

            count--;
        });
    }

    internal Task ConnectionChanged(ServiceStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        switch (notification.ConnectionState)
        {
            case ServiceConnectionState.Connecting:
                Connecting(notification.ServiceConnection);
                break;
            case ServiceConnectionState.Connected:
                Connected(notification.ServiceConnection);
                break;
            case ServiceConnectionState.Disconnected:
                Disconnected(notification.ServiceConnection);
                break;

            default:
                throw new NotImplementedException();
        }

        return Task.CompletedTask;
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ServicePage));

        logger.LogCritical("HELLO");
        count++;
    }
}

