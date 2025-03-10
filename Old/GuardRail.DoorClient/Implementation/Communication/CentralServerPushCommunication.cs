using GuardRail.DeviceLogic.Implementations.Communication;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation.Communication;

public sealed class CentralServerPushCommunication : CoreCentralServerPushCommunication<CentralServerPushCommunication>
{
    public CentralServerPushCommunication(
        ILogger<CentralServerPushCommunication> logger)
        : base(
            logger)
    {
    }
}