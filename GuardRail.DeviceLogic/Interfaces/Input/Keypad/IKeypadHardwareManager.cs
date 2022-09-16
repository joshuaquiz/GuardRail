namespace GuardRail.DeviceLogic.Interfaces.Input.Keypad;

/// <summary>
/// The low level API for interacting with a keypad.
/// </summary>
public interface IKeypadHardwareManager : IHardwareAsyncInit
{
    /// <summary>
    /// The submit event.
    /// </summary>
    public event Func<string, CancellationToken, ValueTask> Submit;

    /// <summary>
    /// The reset event.
    /// </summary>
    public event Func<CancellationToken, ValueTask> Reset;

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        byte[] address);
}