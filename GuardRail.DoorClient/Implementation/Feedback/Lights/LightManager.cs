using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation.Feedback.Lights;

public sealed class LightManager : CoreLightManager<LightManager>
{
    public LightManager(
        ILightConfiguration lightConfiguration,
        ILogger<LightManager> logger,
        ILightHardwareManager lightHardwareManager)
        : base(
            lightConfiguration,
            lightHardwareManager,
            logger)
    {
    }
}