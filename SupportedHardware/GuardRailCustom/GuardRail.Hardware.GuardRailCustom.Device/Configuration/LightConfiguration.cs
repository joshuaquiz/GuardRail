using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;

namespace GuardRail.Hardware.GuardRailCustom.Device.Configuration;

public sealed class LightConfiguration : ILightConfiguration<int>
{
    /// <inheritdoc />
    public int RedLightAddress { get; set; }

    /// <inheritdoc />
    public int GreenLightAddress { get; set; }
}