using System.Threading.Tasks;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;
using GuardRail.DeviceLogic.Implementations.Input;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc;

public sealed class NfcInput : CoreNfcInput<NfcInput, NfcConfiguration>
{
    public NfcInput(NfcConfiguration nfcConfiguration,
        INfcHardwareManager nfcManager,
        ICentralServerCommunication centralServerCommunication,
        ILogger<NfcInput> logger)
        : base(
            nfcConfiguration,
            nfcManager,
            centralServerCommunication,
            logger)
    {
    }

    public override async ValueTask DisposeAsync()
    {
        if (NfcHardwareManager is not null)
        {
            await NfcHardwareManager.DisposeAddressAsync(NfcConfiguration.SerialPort);
        }
    }
}