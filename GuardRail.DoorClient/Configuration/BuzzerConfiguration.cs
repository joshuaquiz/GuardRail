using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DoorClient.Helpers;
using System.ComponentModel;

namespace GuardRail.DoorClient.Configuration;

[TypeConverter(typeof(IntToByteArrayConverter))]
public sealed class BuzzerConfiguration : IBuzzerConfiguration
{
    /// <inheritdoc />
    public byte[] BuzzerAddress { get; set; }
}