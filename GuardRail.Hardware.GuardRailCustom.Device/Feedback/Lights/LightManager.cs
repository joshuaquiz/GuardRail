using GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Lights;

public sealed class LightManager : CoreLightManager<LightManager, int>
{
    public LightManager(
        ILightConfiguration<int> lightConfiguration,
        ILightHardwareManager<int> lightManager,
        ILogger<LightManager> logger)
        : base(
            lightConfiguration,
            lightManager,
            logger)
    {
    }
}