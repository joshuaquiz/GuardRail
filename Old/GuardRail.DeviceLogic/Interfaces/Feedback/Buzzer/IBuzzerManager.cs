using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;

/// <summary>
/// The high level API for interacting with a feedback buzzer.
/// </summary>
public interface IBuzzerManager : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Turns on a buzzer on.
    /// </summary>
    /// <param name="duration">The duration to leave the buzzer on.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to turn the buzzer on.</returns>
    public ValueTask BuzzAsync(
        TimeSpan duration,
        CancellationToken cancellationToken);
}