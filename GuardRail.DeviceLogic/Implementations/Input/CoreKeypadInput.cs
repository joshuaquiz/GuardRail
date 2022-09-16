using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;
using System.Text;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.DeviceLogic.Models;

namespace GuardRail.DeviceLogic.Implementations.Input;

public abstract class CoreKeypadInput<TKeypadInput, TKeypadConfiguration> : IKeypadInput
    where TKeypadInput : CoreKeypadInput<TKeypadInput, TKeypadConfiguration>
    where TKeypadConfiguration : IKeypadConfiguration
{
    protected readonly TKeypadConfiguration KeypadConfiguration;
    protected readonly IKeypadHardwareManager KeypadHardwareManager;
    protected readonly ICentralServerCommunication CentralServerCommunication;
    protected readonly ILogger<TKeypadInput> Logger;

    protected CoreKeypadInput(
        TKeypadConfiguration keypadConfiguration,
        IKeypadHardwareManager keypadHardwareManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<TKeypadInput> logger)
    {
        KeypadConfiguration = keypadConfiguration;
        KeypadHardwareManager = keypadHardwareManager;
        CentralServerCommunication = centralServerCommunication;
        Logger = logger;
        KeypadHardwareManager.Reset += OnKeypadReset;
        KeypadHardwareManager.Submit += OnKeypadSubmit;
    }

    /// <inheritdoc />
    public virtual ValueTask OnKeypadReset(
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public virtual async ValueTask OnKeypadSubmit(
        string inputData,
        CancellationToken cancellationToken) =>
        await CentralServerCommunication.SendDataAsync(
            new UnLockRequest
            {
                DoorId = DeviceConstants.DeviceId,
                UnlockRequestType = UnlockRequestType.Keypad,
                Data = Encoding.UTF8.GetBytes(inputData)
            },
            cancellationToken);

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}