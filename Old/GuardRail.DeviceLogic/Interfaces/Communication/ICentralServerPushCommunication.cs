using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GuardRail.DeviceLogic.Interfaces.Communication;

/// <summary>
/// Defines handling communication from the central server.
/// </summary>
public interface ICentralServerPushCommunication : IHostedService
{
    /// <summary>
    /// Configures a handler for certain data types.
    /// </summary>
    /// <typeparam name="T">The data type to trigger this handler.</typeparam>
    /// <param name="handler">The handler function for the data type.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to register the receiver.</returns>
    public ValueTask ConfigureDataReceiverAsync<T>(
        Func<T?, CancellationToken, ValueTask> handler) where T : class;

    /// <summary>
    /// Attempts to setup the UDP client.
    /// </summary>
    /// <param name="udpClient">The value to use for the client.</param>
    void SetUdpClient(
        UdpClient? udpClient);

    /// <summary>
    /// Attempts to get the UDP client.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to get the client.</returns>
    ValueTask<UdpClient?> GetUdpClient(
        CancellationToken cancellationToken);
}