using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for Screens.
/// </summary>
public static class ScreenSetup
{
    /// <summary>
    /// Adds implementations for <see cref="IScreenConfiguration"/>, <see cref="IScreenHardwareManager"/>, and <see cref="IScreenManager"/> to manage and control a hardware Screen.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Screen".
    /// </remarks>
    /// <typeparam name="TScreenConfiguration">Must be a class and implement <see cref="IScreenConfiguration"/>.</typeparam>
    /// <typeparam name="TScreenHardwareManager">Must be a class and implement <see cref="IScreenHardwareManager"/>.</typeparam>
    /// <typeparam name="TScreenManager">Must be a class and implement <see cref="IScreenManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddScreen<TScreenConfiguration, TScreenHardwareManager, TScreenManager>(
            this IServiceCollection services,
            IConfiguration configuration)
        // where TScreenConfiguration : class, IScreenConfiguration
        //where TScreenHardwareManager : class, IScreenHardwareManager
        where TScreenManager : class, IScreenManager =>
        services
            //.AddSingleton(_ => configuration.GetSection("Screen").Get<TScreenConfiguration>())
            //.AddSingleton<IScreenConfiguration, TScreenConfiguration>()
            //.AddSingleton<IScreenHardwareManager, TScreenHardwareManager>()
            .AddSingleton<IScreenManager, TScreenManager>();

    /// <summary>
    /// Adds an empty <see cref="IScreenManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyScreen(
        this IServiceCollection services) =>
        services
            .AddSingleton<IScreenManager, EmptyScreenManager>();
}