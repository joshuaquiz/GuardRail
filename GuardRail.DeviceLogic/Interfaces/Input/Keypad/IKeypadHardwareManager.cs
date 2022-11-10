using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input.Keypad;

/// <summary>
/// The low level API for interacting with a keypad.
/// </summary>
/// <typeparam name="T">The type used for the keypad's addresses.</typeparam>
public interface IKeypadHardwareManager<in T> : IAsyncInit
{
    /// <summary>
    /// The submit event.
    /// </summary>
    public event Func<string, CancellationToken, ValueTask>? Submit;

    /// <summary>
    /// The reset event.
    /// </summary>
    public event Func<CancellationToken, ValueTask>? Reset;

    /// <summary>
    /// Triggered when the timer detects no activity for the configured duration.
    /// </summary>
    void TimerTick();

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        T address);
}