using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for buzzers.
/// </summary>
public static class BuzzerSetup
{
    /// <summary>
    /// Adds implementations for <see cref="IBuzzerConfiguration{TBuzzerConfigurationType}"/>, <see cref="IBuzzerHardwareManager{TBuzzerConfigurationType}"/>, and <see cref="IBuzzerManager"/> to manage and control a hardware buzzer.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Buzzer".
    /// </remarks>
    /// <typeparam name="TBuzzerConfiguration">Must be a class and implement <see cref="IBuzzerConfiguration{TBuzzerConfigurationType}"/>.</typeparam>
    /// <typeparam name="TBuzzerConfigurationType">The address type for the <see cref="IBuzzerConfiguration{TBuzzerConfigurationType}"/>.</typeparam>
    /// <typeparam name="TBuzzerHardwareManager">Must be a class and implement <see cref="IBuzzerHardwareManager{TBuzzerConfigurationType}"/>.</typeparam>
    /// <typeparam name="TBuzzerManager">Must be a class and implement <see cref="IBuzzerManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddBuzzer<TBuzzerConfiguration, TBuzzerConfigurationType, TBuzzerHardwareManager, TBuzzerManager>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TBuzzerConfiguration : class, IBuzzerConfiguration<TBuzzerConfigurationType>, new()
        where TBuzzerHardwareManager : class, IBuzzerHardwareManager<TBuzzerConfigurationType>
        where TBuzzerManager : class, IBuzzerManager =>
        services
            .AddSingleton(_ => configuration.GetSection("Buzzer").Get<TBuzzerConfiguration>() ?? new TBuzzerConfiguration())
            .AddSingleton<IBuzzerConfiguration<TBuzzerConfigurationType>>(x => x.GetRequiredService<TBuzzerConfiguration>())
            .AddSingleton<TBuzzerHardwareManager>()
            .AddSingleton<IBuzzerHardwareManager<TBuzzerConfigurationType>>(s => s.GetRequiredService<TBuzzerHardwareManager>())
            .AddSingleton<IAsyncInit>(s => s.GetRequiredService<TBuzzerHardwareManager>())
            .AddSingleton<IBuzzerManager, TBuzzerManager>();

    /// <summary>
    /// Adds an empty <see cref="IBuzzerManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyBuzzer(
        this IServiceCollection services) =>
        services
            .AddSingleton<IBuzzerManager, EmptyBuzzerManager>();
}