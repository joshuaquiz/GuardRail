using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input;

public interface ISwipeInput
{
    Task OnSubmit(Span<byte> inputData, CancellationToken cancellationToken);
}