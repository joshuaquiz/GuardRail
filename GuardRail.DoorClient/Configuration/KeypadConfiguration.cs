using System;
using System.Collections.Generic;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;

namespace GuardRail.DoorClient.Configuration;

public sealed class KeypadConfiguration : IKeypadConfiguration
{
    /// <inheritdoc />
    public TimeSpan KeypadTimeout { get; set; }

    /// <inheritdoc />
    public char SubmitKey { get; set; }

    public List<int> ColumnPins { get; set; }

    public List<int> RowPins { get; set; }
}