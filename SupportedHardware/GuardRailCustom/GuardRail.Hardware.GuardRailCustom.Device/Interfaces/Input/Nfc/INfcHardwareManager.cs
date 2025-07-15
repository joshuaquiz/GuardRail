using System;
using System.Collections.Generic;
using System.Threading;

namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;

/// <summary>
/// The low level API for interacting with NFC input.
/// </summary>
public interface INfcHardwareManager : IAsyncInit, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Reads the NFC tags as they are presented from the hardware.
    /// </summary>
    public IAsyncEnumerable<string> ReadTags(CancellationToken cancellationToken);
}