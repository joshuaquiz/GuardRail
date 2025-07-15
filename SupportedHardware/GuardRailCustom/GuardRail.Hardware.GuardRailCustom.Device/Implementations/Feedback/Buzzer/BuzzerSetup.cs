using System;
using System.Linq;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Exceptions;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Buzzer;

/// <summary>
/// Setup extensions for buzzers.
/// </summary>
public static class BuzzerSetup
{
    private const string SectionName = "Buzzer";

    private static TBuzzerConfiguration ValidateAndParseBuzzerConfiguration<TBuzzerConfiguration, TBuzzerConfigurationType>(
        this IConfiguration configuration)
        where TBuzzerConfiguration : class, IBuzzerConfiguration<TBuzzerConfigurationType>, new()
    {
        var section = configuration.GetSection(SectionName);
        var configurationSections = section.GetChildren().ToList();
        if (configurationSections == null
            || configurationSections.Count == 0)
        {
            throw new InvalidConfigurationException(
                SectionName,
                null);
        }

        var value = configurationSections.ToDictionary(x => x.Path, x => x.Value).ToJson();
        try
        {
            var buzzerConfiguration = section.Get<TBuzzerConfiguration?>();
            if (buzzerConfiguration == null)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    " Parsed value is null.");
            }

            if (buzzerConfiguration.BuzzerAddress is <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    $" {nameof(IBuzzerConfiguration<TBuzzerConfigurationType>.BuzzerAddress)} has a value of {buzzerConfiguration.BuzzerAddress}");
            }

            return buzzerConfiguration;
        }
        catch (Exception e)
        {
            throw new InvalidConfigurationException(
                SectionName,
                value,
                $" Could not parse the value as {typeof(TBuzzerConfiguration).FullName}.",
                e);
        }
    }

    /// <summary>
    /// Adds implementations for <see cref="IBuzzerConfiguration{T}"/>, <see cref="IBuzzerHardwareManager{TBuzzerConfigurationType}"/>, and <see cref="IBuzzerManager"/> to manage and control a hardware buzzer.
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
            .AddSingleton(configuration.ValidateAndParseBuzzerConfiguration<TBuzzerConfiguration, TBuzzerConfigurationType>())
            .AddSingleton<TBuzzerHardwareManager>()
            .AddSingleton<TBuzzerManager>()
            .AddSingleton<IBuzzerConfiguration<TBuzzerConfigurationType>>(x => x.GetRequiredService<TBuzzerConfiguration>())
            .AddSingleton<IBuzzerHardwareManager<TBuzzerConfigurationType>>(x => x.GetRequiredService<TBuzzerHardwareManager>())
            .AddSingleton<IBuzzerManager, TBuzzerManager>(x => x.GetRequiredService<TBuzzerManager>())
            .AddSingleton<IAsyncInit, TBuzzerHardwareManager>(x => x.GetRequiredService<TBuzzerHardwareManager>())
            .AddSingleton<IAsyncInit, TBuzzerManager>(x => x.GetRequiredService<TBuzzerManager>());

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