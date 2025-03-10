using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Models;
using GuardRail.DeviceLogic.Models;

namespace GuardRail.DeviceLogic.Implementations.Input;

public abstract class CoreKeypadInput<TKeypadInput, TKeypadConfiguration, TKeypadConfigurationType> : IKeypadInput
    where TKeypadInput : CoreKeypadInput<TKeypadInput, TKeypadConfiguration, TKeypadConfigurationType>
    where TKeypadConfiguration : IKeypadConfiguration<TKeypadConfigurationType>
{
    protected readonly TKeypadConfiguration KeypadConfiguration;
    protected readonly IKeypadHardwareManager<TKeypadConfigurationType>? KeypadHardwareManager;
    protected readonly ICentralServerCommunication CentralServerCommunication;
    protected readonly ILogger<TKeypadInput> Logger;

    protected CoreKeypadInput(
        TKeypadConfiguration keypadConfiguration,
        IKeypadHardwareManager<TKeypadConfigurationType>? keypadHardwareManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<TKeypadInput> logger)
    {
        KeypadConfiguration = keypadConfiguration;
        KeypadHardwareManager = keypadHardwareManager;
        CentralServerCommunication = centralServerCommunication;
        Logger = logger;
    }

    /// <inheritdoc />
    public ValueTask InitAsync()
    {
        if (KeypadHardwareManager != null)
        {
            KeypadHardwareManager.Reset += OnKeypadReset;
            KeypadHardwareManager.Submit += OnKeypadSubmit;
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
                AccessPointGuid = DeviceConstants.DeviceId,
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