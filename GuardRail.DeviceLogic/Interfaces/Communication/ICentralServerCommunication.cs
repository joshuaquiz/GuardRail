using System.Linq.Expressions;

namespace GuardRail.DeviceLogic.Interfaces.Communication;

/// <summary>
/// Defines communication to and from the central server.
/// </summary>
public interface ICentralServerCommunication
{
    /// <summary>
    /// Sends data to the central server.
    /// </summary>
    /// <param name="data">The binary data to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to send the data.</returns>
    public Task SendDataAsync<T>(
        T data,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets data from the central server.
    /// </summary>
    /// <param name="name">The name of the data you are getting.</param>
    /// <param name="extraData">Any extra data related to your request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the work to get the data.</returns>
    public Task<IEnumerable<byte>> GetDataAsync(
        string name,
        IEnumerable<byte> extraData,
        CancellationToken cancellationToken);

    public void ConfigureDataReceiver<T>(
        Expression<Func<T, CancellationToken, Task>> handler);
}