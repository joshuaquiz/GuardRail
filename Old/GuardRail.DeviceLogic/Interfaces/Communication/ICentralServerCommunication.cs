using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Communication;

/// <summary>
/// Defines communication to the central server.
/// </summary>
public interface ICentralServerCommunication : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Sends data to the central server.
    /// </summary>
    /// <param name="path">The path to the data.</param>
    /// <param name="data">The binary data to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to send the data.</returns>
    public ValueTask<bool> SendDataAsync<T>(
        string path,
        T data,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets data from the central server.
    /// </summary>
    /// <param name="path">The path to the data.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to get the data.</returns>
    public ValueTask<Stream> GetDataAsync(
        string path,
        CancellationToken cancellationToken);
}