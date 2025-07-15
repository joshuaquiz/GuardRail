using System;
using System.Linq;
using GuardRail.Hardware.GuardRailCustom.Device.Exceptions;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

/// <summary>
/// Setup extensions for NFCs.
/// </summary>
public static class NfcSetup
{
    private const string SectionName = "Nfc";

    private static T ValidateAndParseNfcConfiguration<T>(
        this IConfiguration configuration)
        where T : class, INfcConfiguration, new()
    {
        var section = configuration.GetSection(SectionName);
        if (!section.GetChildren().Any())
        {
            throw new InvalidConfigurationException(
                SectionName,
                section.Value);
        }

        try
        {
            var nfcConfiguration = section.Get<T?>();
            if (nfcConfiguration == null)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    section.Value,
                    " Parsed value is null.");
            }

            if (nfcConfiguration.BusId <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    section.Value,
                    $" {nameof(INfcConfiguration.BusId)} has a value of {nfcConfiguration.BusId}");
            }

            if (nfcConfiguration.DeviceAddress <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    section.Value,
                    $" {nameof(INfcConfiguration.DeviceAddress)} has a value of {nfcConfiguration.DeviceAddress}");
            }

            return nfcConfiguration;
        }
        catch (Exception e)
        {
            throw new InvalidConfigurationException(
                SectionName,
                section.Value,
                $" Could not parse the value as {typeof(T).FullName}.",
                e);
        }
    }

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
            .AddSingleton(configuration.ValidateAndParseNfcConfiguration<TNfcConfiguration>())
            .AddSingleton<TNfcHardwareManager>()
            .AddSingleton<TNfcInput>()
            .AddSingleton<INfcConfiguration>(x => x.GetRequiredService<TNfcConfiguration>())
            .AddSingleton<INfcHardwareManager>(x => x.GetRequiredService<TNfcHardwareManager>())
            .AddSingleton<INfcInput>(x => x.GetRequiredService<TNfcInput>())
            .AddSingleton<IAsyncInit, TNfcHardwareManager>(x => x.GetRequiredService<TNfcHardwareManager>())
            .AddHostedService<TNfcInput>();

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