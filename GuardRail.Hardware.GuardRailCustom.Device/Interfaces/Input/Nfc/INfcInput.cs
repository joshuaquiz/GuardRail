using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;

public interface INfcInput : IAsyncInit, IAsyncDisposable, IDisposable
{
    public ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken);
}