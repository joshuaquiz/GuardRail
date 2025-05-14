using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input;

public abstract class CoreKeypadInput<TKeypadInput, TKeypadConfiguration, TKeypadConfigurationType>(
    TKeypadConfiguration keypadConfiguration,
    IKeypadHardwareManager<TKeypadConfigurationType>? keypadHardwareManager,
    ILogger<TKeypadInput> logger)
    : IKeypadInput
    where TKeypadInput : CoreKeypadInput<TKeypadInput, TKeypadConfiguration, TKeypadConfigurationType>
    where TKeypadConfiguration : IKeypadConfiguration<TKeypadConfigurationType>
{
    protected readonly TKeypadConfiguration KeypadConfiguration = keypadConfiguration;
    protected readonly IKeypadHardwareManager<TKeypadConfigurationType>? KeypadHardwareManager = keypadHardwareManager;
    protected readonly ILogger<TKeypadInput> Logger = logger;

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
    public virtual ValueTask OnKeypadSubmit(
        string inputData,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    } /* =>
    await CentralServerCommunication.SendDataAsync(
        nameof(UnLockRequest),
        new UnLockRequest
        {
            AccessPointGuid = DeviceConstants.DeviceId,
            UnlockRequestType = UnlockRequestType.Keypad,
            Data = Encoding.UTF8.GetBytes(inputData)
        },
        cancellationToken);*/

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}