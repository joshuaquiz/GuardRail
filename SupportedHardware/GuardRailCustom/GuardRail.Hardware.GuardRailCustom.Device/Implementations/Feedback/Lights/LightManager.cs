using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Lights;

public sealed class LightManager(
    ILightConfiguration<int> lightConfiguration,
    ILightHardwareManager<int> lightManager,
    ILogger<LightManager> logger)
    : CoreLightManager<LightManager, int>(
        lightConfiguration,
        lightManager,
        logger);