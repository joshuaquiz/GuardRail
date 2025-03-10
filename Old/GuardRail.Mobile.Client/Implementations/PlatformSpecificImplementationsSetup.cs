using GuardRail.Mobile.Client.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Mobile.Client.Implementations;

public static class PlatformSpecificImplementationsSetup
{
    public static IServiceCollection AddPlatformSpecificImplementations(
        this IServiceCollection services)
    {
#if ANDROID
        return services
            .AddAndroidOsItems()
            .AddSingleton<IWiFi, AndroidWiFi>();
#else
        return services
            .AddAppleOsItems()
            .AddSingleton<IWiFi, AppleWiFi>();
#endif
    }
}