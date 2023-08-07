using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;

namespace Firebridge.Controller;

public class ServiceMiniView : ContentView
{
    private readonly IControllerContext controllerContext;
    private readonly IServiceConnection serviceConnection;
    private readonly IMediator mediator;
    private readonly Image image;

    public ServiceMiniView(IControllerContext controllerContext, IServiceConnection serviceConnection, IMediator mediator)
    {
        this.serviceConnection = serviceConnection;
        this.controllerContext = controllerContext;
        this.mediator = mediator;

        MenuFlyout menuFlyout = new MenuFlyout();

        ICollection<string> actions = controllerContext.GetActions();

        foreach (var action in actions)
        {
            var actionItem = new MenuFlyoutItem { Text = action };
            actionItem.BindingContext = controllerContext;
            actionItem.Clicked += async (s, args) => await menuItemClicked(s, args);
            menuFlyout.Add(actionItem);
        }

        FlyoutBase.SetContextFlyout(this, menuFlyout);

        image = new Image
        {
            Source = ImageSource.FromUri(new Uri("https://aka.ms/campus.jpg"))
        };

        Content = new VerticalStackLayout
        {
            Children = {
                image
            }
		};
	}

    public async Task SetImage(ImageSource imageSource)
    {
        await Dispatcher.DispatchAsync(() => 
        {
            image.Source = imageSource;
        });
    }

    private async Task menuItemClicked(object sender, EventArgs e)
    {
        if (sender == null || sender is not MenuFlyoutItem)
            return;

        var command = ((MenuFlyoutItem)sender).Text;

        await mediator.Publish(new ControllerActionNotification() { Action = command, Connection = serviceConnection });
    }
}