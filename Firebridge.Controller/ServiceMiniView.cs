namespace Firebridge.Controller;

public class ServiceMiniView : ContentView
{
	public ServiceMiniView()
	{
		Content = new VerticalStackLayout
        {
            Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!" + new Random().Next()
                }
			}
		};
	}
}