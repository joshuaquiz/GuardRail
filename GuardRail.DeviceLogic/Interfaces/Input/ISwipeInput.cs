namespace GuardRail.DeviceLogic.Interfaces.Input;

public interface ISwipeInput
{
    Task OnSubmit(Span<byte> inputData, CancellationToken cancellationToken);
}