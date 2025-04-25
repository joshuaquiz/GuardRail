using GuardRail.DeviceLogic.Implementations.Door;
using GuardRail.DeviceLogic.Interfaces.Door;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for Doors.
/// </summary>
public static class DoorSetup
{
    /*/// <summary>
    /// Adds implementations for <see cref="IDoorConfiguration"/>, <see cref="IDoorHardwareManager"/>, and <see cref="IDoorInput"/> to manage and control a hardware Door.
    /// </summary>
    /// <remarks>
    /// The setting will be pulled form the section called "Door".
    /// </remarks>
    /// <typeparam name="TDoorConfiguration">Must be a class and implement <see cref="IDoorConfiguration"/>.</typeparam>
    /// <typeparam name="TDoorHardwareManager">Must be a class and implement <see cref="IDoorHardwareManager"/>.</typeparam>
    /// <typeparam name="TDoorInput">Must be a class and implement <see cref="IDoorInput"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> that was provided.</returns>
    public static IServiceCollection AddDoor<TDoorConfiguration, TDoorHardwareManager, TDoorInput>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDoorConfiguration : class, IDoorConfiguration
        where TDoorHardwareManager : class, IDoorHardwareManager
        where TDoorInput : class, IDoorInput =>
        services
            .AddSingleton(_ => configuration.GetSection("Door").Get<TDoorConfiguration>())
            .AddSingleton<IDoorConfiguration, TDoorConfiguration>()
            .AddSingleton<IDoorHardwareManager, TDoorHardwareManager>()
            .AddSingleton<IAsyncInit, TDoorHardwareManager>()
            .AddSingleton<IDoorInput, TDoorInput>()
            .AddSingleton<IAsyncInit, TDoorInput>();*/

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