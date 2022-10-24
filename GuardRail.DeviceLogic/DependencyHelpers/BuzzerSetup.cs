using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.DependencyHelpers;

/// <summary>
/// Setup extensions for buzzers.
/// </summary>
public static class BuzzerSetup
{
    /// <summary>
    /// Adds implementations for <see cref="IBuzzerConfiguration"/>, <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer.IBuzzerManager"/>, and <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer.IBuzzerManager"/> to manage and control a hardware buzzer.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Buzzer".
    /// </remarks>
    /// <typeparam name="TBuzzerConfiguration">Must be a class and implement <see cref="IBuzzerConfiguration"/>.</typeparam>
    /// <typeparam name="TBuzzerHardwareManager">Must be a class and implement <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer.IBuzzerManager"/>.</typeparam>
    /// <typeparam name="TBuzzerManager">Must be a class and implement <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer.IBuzzerManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddBuzzer<TBuzzerConfiguration, TBuzzerHardwareManager, TBuzzerManager>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TBuzzerConfiguration : class, IBuzzerConfiguration
        where TBuzzerHardwareManager : class, IBuzzerHardwareManager
        where TBuzzerManager : class, IBuzzerManager =>
        services
            .AddSingleton(_ => configuration.GetSection("Buzzer").Get<TBuzzerConfiguration>())
            .AddSingleton<IBuzzerHardwareManager, TBuzzerHardwareManager>()
            .AddSingleton<IBuzzerManager, TBuzzerManager>();

    /// <summary>
    /// Adds an empty <see cref="GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer.IBuzzerManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyBuzzer(
        this IServiceCollection services) =>
        services
            .AddSingleton<IBuzzerManager, EmptyBuzzerManager>();
}