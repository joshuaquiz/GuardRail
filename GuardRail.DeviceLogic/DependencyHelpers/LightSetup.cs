using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.DependencyHelpers;

/// <summary>
/// Setup extensions for Lights.
/// </summary>
public static class LightSetup
{
    /// <summary>
    /// Adds implementations for <see cref="ILightConfiguration"/>, <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Lights.ILightManager"/>, and <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Lights.ILightManager"/> to manage and control a hardware Light.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Light".
    /// </remarks>
    /// <typeparam name="TLightConfiguration">Must be a class and implement <see cref="ILightConfiguration"/>.</typeparam>
    /// <typeparam name="TLightHardwareManager">Must be a class and implement <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Lights.ILightManager"/>.</typeparam>
    /// <typeparam name="TLightManager">Must be a class and implement <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Lights.ILightManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddLight<TLightConfiguration, TLightHardwareManager, TLightManager>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TLightConfiguration : class, ILightConfiguration
        where TLightHardwareManager : class, ILightHardwareManager
        where TLightManager : class, ILightManager =>
        services
            .AddSingleton(_ => configuration.GetSection("Light").Get<TLightConfiguration>())
            .AddSingleton<ILightHardwareManager, TLightHardwareManager>()
            .AddSingleton<ILightManager, TLightManager>();

    /// <summary>
    /// Adds an empty <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Lights.ILightManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyLight(
        this IServiceCollection services) =>
        services
            .AddSingleton<ILightManager, EmptyLightManager>();
}