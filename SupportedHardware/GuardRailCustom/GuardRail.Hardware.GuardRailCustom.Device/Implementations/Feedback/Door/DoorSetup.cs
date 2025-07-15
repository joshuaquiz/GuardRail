using System;
using System.Linq;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Exceptions;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Door;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Door;

/// <summary>
/// Setup extensions for Doors.
/// </summary>
public static class DoorSetup
{
    private const string SectionName = "Door";

    private static TDoorConfiguration ValidateAndParseDoorConfiguration<TDoorConfiguration, TDoorConfigurationType>(
        this IConfiguration configuration)
        where TDoorConfiguration : class, IDoorConfiguration<TDoorConfigurationType>, new()
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
            var doorConfiguration = section.Get<TDoorConfiguration?>();
            if (doorConfiguration == null)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    " Parsed value is null.");
            }

            if (doorConfiguration.DoorAddress is <= 0)
            {
                throw new InvalidConfigurationException(
                    SectionName,
                    value,
                    $" {nameof(IDoorConfiguration<TDoorConfigurationType>.DoorAddress)} has a value of {doorConfiguration.DoorAddress}");
            }

            return doorConfiguration;
        }
        catch (Exception e)
        {
            throw new InvalidConfigurationException(
                SectionName,
                value,
                $" Could not parse the value as {typeof(TDoorConfiguration).FullName}.",
                e);
        }
    }

    /// <summary>
    /// Adds implementations for <see cref="IDoorConfiguration{T}"/>, <see cref="ILockableDoorHardwareManager{T}"/>, and <see cref="IDoorManager"/> to manage and control a hardware Door.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Door".
    /// </remarks>
    /// <typeparam name="TDoorConfiguration">Must be a class and implement <see cref="IDoorConfiguration{TDoorConfigurationType}"/>.</typeparam>
    /// <typeparam name="TDoorConfigurationType">The address type for the <see cref="IDoorConfiguration{TDoorConfigurationType}"/>.</typeparam>
    /// <typeparam name="TDoorHardwareManager">Must be a class and implement <see cref="ILockableDoorHardwareManager{TDoorConfigurationType}"/>.</typeparam>
    /// <typeparam name="TDoorManager">Must be a class and implement <see cref="IDoorManager"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddDoor<TDoorConfiguration, TDoorConfigurationType, TDoorHardwareManager, TDoorManager>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDoorConfiguration : class, IDoorConfiguration<TDoorConfigurationType>, new()
        where TDoorHardwareManager : class, ILockableDoorHardwareManager<TDoorConfigurationType>
        where TDoorManager : class, IDoorManager =>
        services
            .AddSingleton(configuration.ValidateAndParseDoorConfiguration<TDoorConfiguration, TDoorConfigurationType>())
            .AddSingleton<TDoorHardwareManager>()
            .AddSingleton<TDoorManager>()
            .AddSingleton<IDoorConfiguration<TDoorConfigurationType>>(x => x.GetRequiredService<TDoorConfiguration>())
            .AddSingleton<ILockableDoorHardwareManager<TDoorConfigurationType>>(x => x.GetRequiredService<TDoorHardwareManager>())
            .AddSingleton<IDoorManager>(x => x.GetRequiredService<TDoorManager>())
            .AddSingleton<IAsyncInit, TDoorHardwareManager>(x => x.GetRequiredService<TDoorHardwareManager>())
            .AddSingleton<IAsyncInit, TDoorManager>(x => x.GetRequiredService<TDoorManager>());

    /// <summary>
    /// Adds an empty <see cref="IDoorManager"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddEmptyDoor(
        this IServiceCollection services) =>
        services
            .AddSingleton<IDoorManager, EmptyDoorManager>();
}