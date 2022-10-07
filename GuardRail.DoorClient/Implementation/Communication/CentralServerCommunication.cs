using System;
using GuardRail.DeviceLogic.Implementations.Communication;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation.Communication;

public sealed class CentralServerCommunication : CoreCentralServerCommunication<CentralServerCommunication>
{
    public CentralServerCommunication(
        IServiceProvider serviceProvider,
        ILogger<CentralServerCommunication> logger)
        : base(
            serviceProvider,
            logger)
    {
    }
}