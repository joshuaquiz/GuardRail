namespace GuardRail.Core.Enums;

/// <summary>
/// Ways an unlock can be triggered.
/// </summary>
public enum UnlockTriggerType : byte
{
    UnKnown = 0,

    Keypad = 1,

    Nfc = 2,

    Rfid = 3,

    Swipe = 4,

    Fingerprint = 5,

    Retina = 6
}