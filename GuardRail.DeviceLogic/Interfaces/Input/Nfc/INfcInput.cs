namespace GuardRail.DeviceLogic.Interfaces.Input.Nfc;

public interface INfcInput : IAsyncDisposable, IDisposable
{
    public ValueTask OnSubmit(
        byte[] inputData,
        CancellationToken cancellationToken);
}