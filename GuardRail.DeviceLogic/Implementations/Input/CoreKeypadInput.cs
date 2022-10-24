using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.DeviceLogic.Models;

namespace GuardRail.DeviceLogic.Implementations.Input;

public abstract class CoreKeypadInput<TKeypadInput, TKeypadConfiguration> : IKeypadInput
    where TKeypadInput : CoreKeypadInput<TKeypadInput, TKeypadConfiguration>
    where TKeypadConfiguration : IKeypadConfiguration
{
    protected readonly TKeypadConfiguration KeypadConfiguration;
    protected readonly IKeypadManager? KeypadManager;
    protected readonly ICentralServerCommunication CentralServerCommunication;
    protected readonly ILogger<TKeypadInput> Logger;

    protected CoreKeypadInput(
        TKeypadConfiguration keypadConfiguration,
        IKeypadManager? keypadManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<TKeypadInput> logger)
    {
        KeypadConfiguration = keypadConfiguration;
        KeypadManager = keypadManager;
        CentralServerCommunication = centralServerCommunication;
        Logger = logger;
    }

    /// <inheritdoc />
    public ValueTask InitAsync()
    {
        if (KeypadManager != null)
        {
            KeypadManager.Reset += OnKeypadReset;
            KeypadManager.Submit += OnKeypadSubmit;
        }

        return ValueTask.CompletedTask;
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
            nameof(UnLockRequest),
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