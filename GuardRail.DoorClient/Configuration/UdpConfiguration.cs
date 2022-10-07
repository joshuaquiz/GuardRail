using GuardRail.DeviceLogic.Interfaces.Communication;

namespace GuardRail.DoorClient.Configuration;

public sealed class UdpConfiguration : IUdpConfiguration
{
    /// <inheritdoc />
    public int Port { get; set; }
}