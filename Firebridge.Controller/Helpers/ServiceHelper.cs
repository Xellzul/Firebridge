namespace Firebridge.Controller.Helpers;

public static class ServiceHelper
{
    public static TService GetService<TService>() => Current.GetService<TService>();

    public static IServiceProvider Current => MauiWinUIApplication.Current.Services;
}
