namespace GuardRail.DeviceLogic.Interfaces.Input;

public interface IRfidInput
{
    Task OnSubmit(Span<byte> inputData, CancellationToken cancellationToken);
}