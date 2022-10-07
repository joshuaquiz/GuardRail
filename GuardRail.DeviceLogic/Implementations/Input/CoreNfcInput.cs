using GuardRail.DeviceLogic.Interfaces.Communication;
using Microsoft.Extensions.Logging;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.DeviceLogic.Models;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;

namespace GuardRail.DeviceLogic.Implementations.Input;

public abstract class CoreNfcInput : INfcInput
{
    protected readonly INfcConfiguration NfcConfiguration;
    protected readonly INfcHardwareManager NfcHardwareManager;
    protected readonly ICentralServerCommunication CentralServerCommunication;
    protected readonly ILogger<CoreNfcInput> Logger;

    protected CoreNfcInput(
        INfcConfiguration nfcConfiguration,
        INfcHardwareManager nfcHardwareManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<CoreNfcInput> logger)
    {
        NfcConfiguration = nfcConfiguration;
        NfcHardwareManager = nfcHardwareManager;
        CentralServerCommunication = centralServerCommunication;
        Logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask OnSubmit(
        byte[] inputData,
        CancellationToken cancellationToken) =>
        await CentralServerCommunication.SendDataAsync(
            nameof(UnLockRequest),
            new UnLockRequest
            {
                DoorId = DeviceConstants.DeviceId,
                UnlockRequestType = UnlockRequestType.Nfc,
                Data = inputData
            },
            cancellationToken);

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}