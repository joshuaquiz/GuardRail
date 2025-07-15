using System.Runtime.InteropServices;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools.PInvoke;

/**
 * @struct nfc_target
 * @brief NFC target structure
 */
[StructLayout(LayoutKind.Sequential)]
public struct nfc_target
{
    public nfc_iso14443a_info nti;
    public nfc_modulation nm;
} ;