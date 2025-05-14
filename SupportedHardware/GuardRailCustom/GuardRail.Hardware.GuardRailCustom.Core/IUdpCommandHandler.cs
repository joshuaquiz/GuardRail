using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public interface IUdpCommandHandler
{
    public static string CommandName => null!;

    public ValueTask<string?> HandleCommand(
        string commandBody,
        CancellationToken cancellationToken);
}