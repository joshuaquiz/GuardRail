using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for Lights.
/// </summary>
public static class LightSetup
{
    /// <summary>
    /// Adds implementations for <see cref="ILightConfiguration{TLightConfigurationType}"/>, <see cref="ILightManager"/>, and <see cref="ILightManager"/> to manage and control a hardware Light.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Light".
    /// </remarks>
    /// <typeparam name="TLightConfiguration">Must be a class and implement <see cref="ILightConfiguration{TLightConfigurationType}"/>.</typeparam>
    /// <typeparam name="TLightConfigurationType">The address type for the <see cref="ILightConfiguration{TLightConfigurationType}"/>.</typeparam>
    /// <typeparam name="TLightHardwareManager">Must be a class and implement <see cref="ILightManager"/>.</typeparam>
    /// <typeparam name="TLightManager">Must be a class and implement <see cref="ILightManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddLight<TLightConfiguration, TLightConfigurationType, TLightHardwareManager, TLightManager>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TLightConfiguration : class, ILightConfiguration<TLightConfigurationType>, new()
        where TLightHardwareManager : class, ILightHardwareManager<TLightConfigurationType>
        where TLightManager : class, ILightManager =>
        services
            .AddSingleton(_ => configuration.GetSection("Light").Get<TLightConfiguration>() ?? new TLightConfiguration())
            .AddSingleton<ILightConfiguration<TLightConfigurationType>>(s => s.GetRequiredService<TLightConfiguration>())
            .AddSingleton<TLightHardwareManager>()
            .AddSingleton<ILightHardwareManager<TLightConfigurationType>>(s => s.GetRequiredService<TLightHardwareManager>())
            .AddSingleton<IAsyncInit>(s => s.GetRequiredService<TLightHardwareManager>())
            .AddSingleton<ILightManager, TLightManager>();

    /// <summary>
    /// Adds an empty <see cref="ILightManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyLight(
        this IServiceCollection services) =>
        services
            .AddSingleton<ILightManager, EmptyLightManager>();
}