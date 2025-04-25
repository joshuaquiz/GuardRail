using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public interface IUdpCommandHandler
{
    public static string CommandName { get; } = null!;

    public ValueTask<string?> HandleCommand(
        UdpResponse data,
        CancellationToken cancellationToken);
}