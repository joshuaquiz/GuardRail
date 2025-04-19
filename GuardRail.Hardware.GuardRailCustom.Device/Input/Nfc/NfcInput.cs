using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Configuration;
using GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Communication;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Input.Nfc;

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