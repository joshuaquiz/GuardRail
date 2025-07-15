using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models;
using GuardRail.Hardware.Common;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.CommandHandlers;

public sealed class UnlockRequestCommandHandler(
    IAccessRequestProvider accessRequestProvider,
    ILogger<UnlockRequestCommandHandler> logger)
    : IUdpCommandHandler
{
    public static string CommandName =>
        GuardRailCustomConstants.UdpCommandNames.UnlockRequest;

    public async ValueTask<string?> HandleCommand(
        string commandBody,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Processing {CommandName}: {commandBody}");
        var unlockRequest = commandBody.FromJson<UnlockRequestCommandData>();
        if (unlockRequest == null)
        {
            return null;
        }

        await accessRequestProvider.TriggerRequestAccess(
            unlockRequest,
            cancellationToken);
        return null;
    }
}