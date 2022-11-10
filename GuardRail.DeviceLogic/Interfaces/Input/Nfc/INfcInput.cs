using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input.Nfc;

public interface INfcInput : IAsyncInit, IAsyncDisposable, IDisposable
{
    public ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken);
}