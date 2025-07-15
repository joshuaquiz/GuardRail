using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.CommandHandlers;

public sealed class ConfirmConnectionCommandHandler(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    ILogger<UnlockRequestCommandHandler> logger)
    : IUdpCommandHandler
{
    public static string CommandName =>
        GuardRailCustomConstants.UdpCommandNames.ConfirmConnection;

    public ValueTask<string?> HandleCommand(
        string commandBody,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Processing {CommandName}: {commandBody}");
        return ValueTask.FromResult(
            commandBody == "Confirm"
                ? hardwareDiscoveryPacket.ToJson()
                : null);
    }
}