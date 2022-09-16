namespace GuardRail.DeviceLogic.Interfaces;

public interface IHardwareAsyncInit
{
    /// <summary>
    /// Initialize the hardware.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the work to initialize the hardware.</returns>
    public ValueTask InitAsync();
}