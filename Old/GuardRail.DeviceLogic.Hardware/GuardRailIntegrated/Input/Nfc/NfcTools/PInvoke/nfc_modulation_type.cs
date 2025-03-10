namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools.PInvoke;

/**
 * @enum nfc_modulation_type
 * @brief NFC modulation type enumeration
 */
public enum nfc_modulation_type
{
    NMT_ISO14443A = 1,
    NMT_JEWEL,
    NMT_ISO14443B,
    NMT_ISO14443BI, // pre-ISO14443B aka ISO/IEC 14443 B' or Type B'
    NMT_ISO14443B2SR, // ISO14443-2B ST SRx
    NMT_ISO14443B2CT, // ISO14443-2B ASK CTx
    NMT_FELICA,
    NMT_DEP,
}