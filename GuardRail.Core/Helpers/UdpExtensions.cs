using System;
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
        this T _,
        ILogger<T> logger)
        where T : UdpClient
    {
        _logger = logger;
    }

    public static async Task SendEncryptedData(
        this UdpClient udpClient,
        IPEndPoint endPoint,
        string data,
        string encryptionKey,
        CancellationToken cancellationToken)
    {
        _logger?.LogGuardRailDebug($"Sending {data} to non-client endpoint {endPoint}");
        await udpClient.SendAsync(
            Encoding.UTF8.GetBytes(
                Encryption.Encrypt(
                    data,
                    typeof(Encryption).Assembly.FullName! + encryptionKey)!),
            endPoint,
            cancellationToken);
    }

    public static async Task<(string? Response, IPEndPoint ReceivedFrom)?> ReceiveEncryptedData(
        this UdpClient udpClient,
        string encryptionKey,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await udpClient.ReceiveAsync(cancellationToken);
            var encryptedResponseData = Encoding.UTF8
                .GetString(
                    response.Buffer);
            _logger?.LogGuardRailDebug($"Got {encryptedResponseData} from {response.RemoteEndPoint}");
            var decryptedString = Encryption.Decrypt(
                encryptedResponseData,
                typeof(Encryption).Assembly.FullName! + encryptionKey)!;
            _logger?.LogGuardRailDebug($"Got {decryptedString} from {response.RemoteEndPoint}");
            return (Response: decryptedString, ReceivedFrom: response.RemoteEndPoint);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}