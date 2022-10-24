using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input.Nfc;

public interface INfcInput : IAsyncDisposable, IDisposable
{
    public ValueTask OnSubmit(
        byte[] inputData,
        CancellationToken cancellationToken);
}