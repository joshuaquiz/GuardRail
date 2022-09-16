namespace GuardRail.DeviceLogic.Interfaces.Feedback.Lights;

/// <summary>
/// The low level API for interacting with feedback lights.
/// </summary>
public interface ILightHardwareManager : IHardwareAsyncInit
{
    /// <summary>
    /// Turns on a light.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to turn the light on.</returns>
    public ValueTask TurnLightOnAsync(
        byte[] address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Turns off a light.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to turn the light off.</returns>
    public ValueTask TurnLightOffAsync(
        byte[] address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        byte[] address);
}