using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Communication;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input;

public abstract class CoreNfcInput<TNfcInput, TNfcConfiguration> : INfcInput
    where TNfcInput : CoreNfcInput<TNfcInput, TNfcConfiguration>
    where TNfcConfiguration : INfcConfiguration
{
    protected readonly TNfcConfiguration NfcConfiguration;
    protected readonly INfcHardwareManager? NfcHardwareManager;
    protected readonly ICentralServerCommunication CentralServerCommunication;
    protected readonly ILogger<TNfcInput> Logger;

    protected CoreNfcInput(
        TNfcConfiguration nfcConfiguration,
        INfcHardwareManager? nfcHardwareManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<TNfcInput> logger)
    {
        NfcConfiguration = nfcConfiguration;
        NfcHardwareManager = nfcHardwareManager;
        CentralServerCommunication = centralServerCommunication;
        Logger = logger;
    }

    /// <inheritdoc />
    public ValueTask InitAsync()
    {
        if (NfcHardwareManager != null)
        {
            NfcHardwareManager.Submit += OnNfcSubmit;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken) =>
        await CentralServerCommunication.SendDataAsync(
            nameof(UnLockRequest),
            new UnLockRequest
            {
                AccessPointGuid = DeviceConstants.DeviceId,
                UnlockRequestType = UnlockRequestType.Nfc,
                Data = Encoding.UTF8.GetBytes(inputData)
            },
            cancellationToken);

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}