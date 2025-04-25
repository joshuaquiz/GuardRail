using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GuardRail.Core.Helpers;

public static class UdpExtensions
{
    private static ILogger<UdpClient>? _logger;

    public static void ConfigureEncryptedTrafficLogging<T>(
        this T udpClient,
        ILogger<T> logger)
        where T : UdpClient
    {
        _logger = logger;
    }

    public static async Task SendEncryptedData(
        this UdpClient udpClient,
        string data,
        CancellationToken cancellationToken)
    {
        _logger?.LogGuardRailDebug($"Sending {data} to {udpClient.Client.RemoteEndPoint}");
        await udpClient.SendAsync(
            Encoding.UTF8.GetBytes(
                Encryption.Encrypt(
                    data,
                    typeof(Encryption).Assembly.FullName!)!),
            cancellationToken);
    }

    public static async Task SendEncryptedData(
        this UdpClient udpClient,
        IPEndPoint endPoint,
        string data,
        CancellationToken cancellationToken)
    {
        _logger?.LogGuardRailDebug($"Sending {data} to non-client endpoint {endPoint}");
        await udpClient.SendAsync(
            Encoding.UTF8.GetBytes(
                Encryption.Encrypt(
                    data,
                    typeof(Encryption).Assembly.FullName!)!),
            endPoint,
            cancellationToken);
    }

    public static async Task<(string Response, IPEndPoint ReceivedFrom)> ReceiveEncryptedData(
        this UdpClient udpClient,
        CancellationToken cancellationToken)
    {
        _logger?.LogGuardRailDebug($"Waiting for data from {udpClient.Client.RemoteEndPoint}");
        var response = await udpClient.ReceiveAsync(cancellationToken);
        var encryptedResponseData = Encoding.UTF8
            .GetString(
                response.Buffer);
        _logger?.LogGuardRailDebug($"Got {encryptedResponseData} from {udpClient.Client.RemoteEndPoint}");
        var decryptedString = Encryption.Decrypt(
            encryptedResponseData,
            typeof(Encryption).Assembly.FullName!)!;
        _logger?.LogGuardRailDebug($"Got {decryptedString} from {udpClient.Client.RemoteEndPoint}");
        return (Response: decryptedString, ReceivedFrom: response.RemoteEndPoint);
    }
}