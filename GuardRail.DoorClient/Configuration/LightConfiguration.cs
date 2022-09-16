using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DoorClient.Helpers;
using System.ComponentModel;

namespace GuardRail.DoorClient.Configuration;

[TypeConverter(typeof(IntToByteArrayConverter))]
public sealed class LightConfiguration : ILightConfiguration
{
    /// <inheritdoc />
    public byte[] RedLightAddress { get; set; }

    /// <inheritdoc />
    public byte[] GreenLightAddress { get; set; }
}