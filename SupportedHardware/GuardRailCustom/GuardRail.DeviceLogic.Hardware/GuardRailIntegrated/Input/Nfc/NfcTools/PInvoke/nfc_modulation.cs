using System.Runtime.InteropServices;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools.PInvoke;

/**
 * @struct nfc_modulation
 * @brief NFC modulation structure
 */
[StructLayout(LayoutKind.Sequential)]
public struct nfc_modulation
{
    public nfc_modulation_type nmt;
    public nfc_baud_rate nbr;
}