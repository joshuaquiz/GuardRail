using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Feedback.Lights;

/// <summary>
/// The high level API for interacting with feedback lights.
/// </summary>
public interface ILightManager : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Turns on a red light.
    /// </summary>
    /// <param name="duration">The duration to leave the light on.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to turn the red light on.</returns>
    public ValueTask TurnOnRedLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Turns on a green light.
    /// </summary>
    /// <param name="duration">The duration to leave the light on.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to turn the green light on.</returns>
    public ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken);
}