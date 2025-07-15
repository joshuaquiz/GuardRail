using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Feedback.Buzzer;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Feedback.Lights;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Device.Gpio;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;

/// <summary>
/// Setup extensions for GuardRail integrated hardware setup.
/// </summary>
public static class GuardRailIntegratedSetup
{
    public static IServiceCollection AddGuardRailIntegratedHardware(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddSingleton(_ => new GpioController())
            .AddSingleton<IGpio, Gpio>()
            .AddBuzzer<BuzzerConfiguration, int, BuzzerHardwareManager, BuzzerManager>(configuration)
            .AddLight<LightConfiguration, int, LightHardwareManager, LightManager>(configuration)
            .AddKeypad<KeypadConfiguration, int, KeypadHardwareManager, KeypadInput>(configuration)
            .AddNfc<NfcConfiguration, NfcHardwareManager, NfcInput>(configuration)
            .AddEmptyNfc()
            .AddEmptyScreen()
            .AddEmptyDoor();
}