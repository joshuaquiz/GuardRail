#if !DEBUG
using System.Device.Gpio;
using GuardRail.Hardware.GuardRailCustom.Device.Configuration;
using GuardRail.Hardware.GuardRailCustom.Device.Feedback.Buzzer;
using GuardRail.Hardware.GuardRailCustom.Device.Feedback.Door;
using GuardRail.Hardware.GuardRailCustom.Device.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Input;
using GuardRail.Hardware.GuardRailCustom.Device.Input.Nfc;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.DependencyHelpers;

/// <summary>
/// Setup extensions for GuardRail integrated hardware setup.
/// </summary>
public static class GuardRailIntegratedSetup
{
    public static IServiceCollection AddGuardRailIntegratedHardware(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
#if DEBUG
            .AddEmptyBuzzer()
            .AddEmptyLight()
            .AddEmptyKeypad()
            .AddEmptyNfc()
            .AddEmptyDoor()
            .AddEmptyKeypad()
            .AddEmptyNfc()
            .AddEmptyScreen();
#else
            .AddSingleton(_ => new GpioController())
            .AddSingleton<IGpio, Gpio>()
            .AddBuzzer<BuzzerConfiguration, int, BuzzerHardwareManager, BuzzerManager>(configuration)
            .AddLight<LightConfiguration, int, LightHardwareManager, LightManager>(configuration)
            //.AddKeypad<KeypadConfiguration, int, KeypadHardwareManager, KeypadInput>(configuration)
            //.AddNfc<NfcConfiguration, NfcHardwareManager, NfcInput>(configuration)
            .AddDoor<DoorConfiguration, int, LockableDoorHardwareManager, DoorManager>(configuration)
            .AddEmptyKeypad()
            .AddEmptyNfc()
            .AddEmptyScreen();
#endif
}