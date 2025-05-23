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
    /// <summary>
    /// Adds implementations for <see cref="IDoorConfiguration{T}"/>, <see cref="ILockableDoorHardwareManager{T}"/>, and <see cref="IDoorManager"/> to manage and control a hardware Door.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Door".
    /// </remarks>
    /// <typeparam name="TDoorConfiguration">Must be a class and implement <see cref="IDoorConfiguration{TDoorConfigurationType}"/>.</typeparam>
    /// <typeparam name="TDoorConfigurationType">The address type for the <see cref="IDoorConfiguration{TBuzzerConfigurationType}"/>.</typeparam>
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
            .AddSingleton(configuration.GetSection("Door").Get<TDoorConfiguration>() ?? new TDoorConfiguration())
            .AddSingleton<IDoorConfiguration<TDoorConfigurationType>, TDoorConfiguration>()
            .AddSingleton<TDoorHardwareManager>()
            .AddSingleton<ILockableDoorHardwareManager<TDoorConfigurationType>, TDoorHardwareManager>()
            .AddSingleton<IAsyncInit, TDoorHardwareManager>()
            .AddSingleton<IAsyncInit, TDoorManager>()
            .AddSingleton<IDoorManager, TDoorManager>();

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