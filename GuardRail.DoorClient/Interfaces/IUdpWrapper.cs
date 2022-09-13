using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces;

/// <summary>
/// Wraps a <see cref="UdpClient"/> that may or may not exist.
/// </summary>
public interface IUdpWrapper
{
    /// <summary>
    /// Set up update the <see cref="UdpClient"/>.
    /// </summary>
    /// <param name="udpClient">The new client.</param>
    void SetUdpClient(UdpClient udpClient);

    /// <summary>
    /// Get the <see cref="UdpClient"/>.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to stop waiting for a <see cref="UdpClient"/>.</param>
    /// <returns><see cref="UdpClient"/></returns>
    Task<UdpClient> GetUdpClient(CancellationToken cancellationToken);
}