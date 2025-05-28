using System;
using System.Linq;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Exceptions;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Lights;

/// <summary>
/// Setup extensions for Lights.
/// </summary>
public static class LightSetup
{
    private const string SectionName = "Light";

    private static TLightConfiguration ValidateAndParseLightConfiguration<TLightConfiguration, TLightConfigurationType>(
        this IConfiguration configuration)
        where TLightConfiguration : class, ILightConfiguration<TLightConfigurationType>, new()
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
            var lightConfiguration = section.Get<TLightConfiguration?>();
            if (lightConfiguration == null)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    " Parsed value is null.");
            }

            if (lightConfiguration.RedLightAddress is <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    $" {nameof(ILightConfiguration<TLightConfigurationType>.RedLightAddress)} has a value of {lightConfiguration.RedLightAddress}");
            }

            if (lightConfiguration.GreenLightAddress is <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    $" {nameof(ILightConfiguration<TLightConfigurationType>.GreenLightAddress)} has a value of {lightConfiguration.GreenLightAddress}");
            }

            return lightConfiguration;
        }
        catch (Exception e)
        {
            throw new InvalidConfigurationException(
                SectionName,
                value,
                $" Could not parse the value as {typeof(TLightConfiguration).FullName}.",
                e);
        }
    }

    /// <summary>
    /// Adds implementations for <see cref="ILightConfiguration{T}"/>, <see cref="ILightManager"/>, and <see cref="ILightManager"/> to manage and control a hardware Light.
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
            .AddSingleton(configuration.ValidateAndParseLightConfiguration<TLightConfiguration, TLightConfigurationType>())
            .AddSingleton<TLightHardwareManager>()
            .AddSingleton<TLightManager>()
            .AddSingleton<ILightConfiguration<TLightConfigurationType>>(x => x.GetRequiredService<TLightConfiguration>())
            .AddSingleton<ILightHardwareManager<TLightConfigurationType>>(x => x.GetRequiredService<TLightHardwareManager>())
            .AddSingleton<ILightManager>(x => x.GetRequiredService<TLightManager>())
            .AddSingleton<IAsyncInit, TLightHardwareManager>(x => x.GetRequiredService<TLightHardwareManager>())
            .AddSingleton<IAsyncInit, TLightManager>(x => x.GetRequiredService<TLightManager>());

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