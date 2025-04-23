using System.Runtime.InteropServices;

namespace GuardRail.Hardware.GuardRailCustom.Device.Input.Nfc.NfcTools.PInvoke;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct nfc_user_defined_device
{
    public string name;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.DEVICE_NAME_LENGTH)]
    public string connstring;
    public bool optional;
}