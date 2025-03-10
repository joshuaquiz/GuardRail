using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input;

public interface IRfidInput
{
    Task OnSubmit(Span<byte> inputData, CancellationToken cancellationToken);
}