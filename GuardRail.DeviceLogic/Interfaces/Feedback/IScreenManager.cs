using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Feedback;

/// <summary>
/// Feedback options with using a display.
/// </summary>
public interface IScreenManager : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Displays a message on the screen.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="duration">How long to show the message.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to display the message.</returns>
    public ValueTask DisplayMessageAsync(
        string message,
        TimeSpan duration,
        CancellationToken cancellationToken);
}