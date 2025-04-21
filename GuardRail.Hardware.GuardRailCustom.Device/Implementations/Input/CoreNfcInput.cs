using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Communication;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input;

public abstract class CoreNfcInput<TNfcInput, TNfcConfiguration>(
    TNfcConfiguration nfcConfiguration,
    INfcHardwareManager? nfcHardwareManager,
    ICentralServerCommunication centralServerCommunication,
    ILogger<TNfcInput> logger)
    : INfcInput
    where TNfcInput : CoreNfcInput<TNfcInput, TNfcConfiguration>
    where TNfcConfiguration : INfcConfiguration
{
    /// <inheritdoc />
    public ValueTask InitAsync()
    {
        if (nfcHardwareManager != null)
        {
            nfcHardwareManager.Submit += OnNfcSubmit;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public virtual ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }/* =>
        await centralServerCommunication.SendDataAsync(
            nameof(UnLockRequest),
            new UnLockRequest
            {
                AccessPointGuid = DeviceConstants.DeviceId,
                UnlockRequestType = UnlockRequestType.Nfc,
                Data = Encoding.UTF8.GetBytes(inputData)
            },
            cancellationToken);*/

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}