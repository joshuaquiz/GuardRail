using System;
using System.Runtime.InteropServices;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools.PInvoke;

public struct pn53x_data
{
    public pn53x_type type;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 22)]
    public string firmware_text;
    public pn53x_power_mode power_mode;
    public pn53x_operating_mode operating_mode;
    public IntPtr current_target; // Assuming nfc_target is a class or struct
    public pn532_sam_mode sam_mode;
    public IntPtr io; // Assuming pn53x_io is a class or struct
    public byte last_status_byte;
    public byte ui8TxBits;
    public byte ui8Parameters;
    public byte last_command;
    public short timer_correction;
    public ushort timer_prescaler;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = PN53X_CACHE_REGISTER_SIZE)]
    public byte[] wb_data;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = PN53X_CACHE_REGISTER_SIZE)]
    public byte[] wb_mask;
    public bool wb_trigged;
    public int timeout_command;
    public int timeout_atr;
    public int timeout_communication;
    public IntPtr supported_modulation_as_initiator; // Assuming nfc_modulation_type is an enum
    public IntPtr supported_modulation_as_target; // Assuming nfc_modulation_type is an enum
    public bool progressive_field;

    // Add PN53X_CACHE_REGISTER_SIZE constant based on your actual implementation
    private const int PN53X_CACHE_REGISTER_SIZE = 32;
}

/**
 * @struct nfc_device
 * @brief NFC device information
 */
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public ref struct nfc_device
{
    public ref nfc_context context;
    public IntPtr driver;
    public IntPtr driver_data;
    public IntPtr chip_data;

    /** Device name string, including device wrapper firmware */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.DEVICE_NAME_LENGTH)]
    public string name;
    /** Device connection string */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.NFC_BUFSIZE_CONNSTRING)]
    public string connstring;
    /** Is the CRC automaticly added, checked and removed from the frames */
    public bool bCrc;
    /** Does the chip handle parity bits, all parities are handled as data */
    public bool bPar;
    /** Should the chip handle frames encapsulation and chaining */
    public bool bEasyFraming;
    /** Should the chip try forever on select? */
    public bool bInfiniteSelect;
    /** Should the chip switch automatically activate ISO14443-4 when
        selecting tags supporting it? */
    public bool bAutoIso14443_4;
    /** Supported modulation encoded in a byte */
    public byte btSupportByte;
    /** Last reported error */
    public int last_error;
}

//struct nfc_device {
//  const nfc_context *context;
//  const struct nfc_driver *driver;
//  void *driver_data;
//  void *chip_data;

//  /** Device name string, including device wrapper firmware */
//  char    name[DEVICE_NAME_LENGTH];
//  /** Device connection string */
//  nfc_connstring connstring;
//  /** Is the CRC automaticly added, checked and removed from the frames */
//  bool    bCrc;
//  /** Does the chip handle parity bits, all parities are handled as data */
//  bool    bPar;
//  /** Should the chip handle frames encapsulation and chaining */
//  bool    bEasyFraming;
//  /** Should the chip try forever on select? */
//  bool    bInfiniteSelect;
//  /** Should the chip switch automatically activate ISO14443-4 when
//      selecting tags supporting it? */
//  bool    bAutoIso14443_4;
//  /** Supported modulation encoded in a byte */
//  uint8_t  btSupportByte;
//  /** Last reported error */
//  int     last_error;
//};