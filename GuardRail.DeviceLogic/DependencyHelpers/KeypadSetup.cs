using GuardRail.DeviceLogic.Implementations.Input;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.DependencyHelpers;

/// <summary>
/// Setup extensions for Keypads.
/// </summary>
public static class KeypadSetup
{
    /// <summary>
    /// Adds implementations for <see cref="IKeypadConfiguration"/>, <see cref="IKeypadManager"/>, and <see cref="IKeypadInput"/> to manage and control a hardware Keypad.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Keypad".
    /// </remarks>
    /// <typeparam name="TKeypadConfiguration">Must be a class and implement <see cref="IKeypadConfiguration"/>.</typeparam>
    /// <typeparam name="TKeypadHardwareManager">Must be a class and implement <see cref="IKeypadManager"/>.</typeparam>
    /// <typeparam name="TKeypadInput">Must be a class and implement <see cref="IKeypadInput"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddKeypad<TKeypadConfiguration, TKeypadHardwareManager, TKeypadInput>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TKeypadConfiguration : class, IKeypadConfiguration
        where TKeypadHardwareManager : class, IKeypadManager
        where TKeypadInput : class, IKeypadInput =>
        services
            .AddSingleton(_ => configuration.GetSection("Keypad").Get<TKeypadConfiguration>())
            .AddSingleton<IKeypadManager, TKeypadHardwareManager>()
            .AddSingleton<IKeypadInput, TKeypadInput>();

    /// <summary>
    /// Adds an empty <see cref="IKeypadInput"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyKeypad(
        this IServiceCollection services) =>
        services
            .AddSingleton<IKeypadInput, EmptyKeypadInput>();
}