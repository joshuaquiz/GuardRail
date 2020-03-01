using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Definitions
{
    /// <summary>
    /// The event handler for an access requested event.
    /// </summary>
    /// <param name="sender">The object that triggered the request.</param>
    /// <param name="e">Information about the event</param>
    /// <param name="cancellationToken">A CancellationToken.</param>
    /// <returns>Returns if access was granted.</returns>
    public delegate Task<bool> AccessRequestedEventHandlerAsync(
        object sender,
        AccessRequestedEventArgs e,
        CancellationToken cancellationToken);
}