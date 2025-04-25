using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Feedback.Lights;

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