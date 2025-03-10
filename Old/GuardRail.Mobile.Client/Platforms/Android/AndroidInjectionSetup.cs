using Android.Content;
using Android.Net.Wifi;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Mobile.Client;

public static class AndroidInjectionSetup
{
    public static IServiceCollection AddAndroidOsItems(
        this IServiceCollection services) =>
        services
            .AddSingleton(Android.App.Application.Context)
            .AddSingleton(serviceProvider => (serviceProvider.GetRequiredService<Context>().GetSystemService(Context.WifiService) as WifiManager)!)
            .AddSingleton(serviceProvider => (serviceProvider.GetRequiredService<Context>().GetSystemService(Context.ConnectivityService) as Android.Net.ConnectivityManager)!);
}