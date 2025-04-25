using GuardRail.DeviceLogic.Implementations.Input;
using GuardRail.DeviceLogic.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for NFCs.
/// </summary>
public static class NfcSetup
{
    /// <summary>
    /// Adds implementations for <see cref="INfcConfiguration"/>, <see cref="INfcHardwareManager"/>, and <see cref="INfcInput"/> to manage and control a hardware Nfc.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Nfc".
    /// </remarks>
    /// <typeparam name="TNfcConfiguration">Must be a class and implement <see cref="INfcConfiguration"/>.</typeparam>
    /// <typeparam name="TNfcHardwareManager">Must be a class and implement <see cref="INfcHardwareManager"/>.</typeparam>
    /// <typeparam name="TNfcInput">Must be a class and implement <see cref="INfcInput"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddNfc<TNfcConfiguration, TNfcHardwareManager, TNfcInput>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TNfcConfiguration : class, INfcConfiguration, new()
        where TNfcHardwareManager : class, INfcHardwareManager
        where TNfcInput : class, INfcInput =>
        services
            .AddSingleton(_ => configuration.GetSection("Nfc").Get<TNfcConfiguration>() ?? new TNfcConfiguration())
            .AddSingleton<INfcConfiguration>(s => s.GetRequiredService<TNfcConfiguration>())
            .AddSingleton<TNfcHardwareManager>()
            .AddSingleton<INfcHardwareManager>(s => s.GetRequiredService<TNfcHardwareManager>())
            .AddSingleton<IAsyncInit>(s => s.GetRequiredService<TNfcHardwareManager>())
            .AddSingleton<TNfcInput>()
            .AddSingleton<INfcInput>(s => s.GetRequiredService<TNfcInput>())
            .AddSingleton<IAsyncInit>(s => s.GetRequiredService<TNfcInput>());

    /// <summary>
    /// Adds an empty <see cref="INfcInput"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyNfc(
        this IServiceCollection services) =>
        services
            .AddSingleton<INfcInput, EmptyNfcInput>();
}