using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Lights;

public sealed class LightConfiguration : ILightConfiguration<int>
{
    /// <inheritdoc />
    public int RedLightAddress { get; set; }

    /// <inheritdoc />
    public int GreenLightAddress { get; set; }
}