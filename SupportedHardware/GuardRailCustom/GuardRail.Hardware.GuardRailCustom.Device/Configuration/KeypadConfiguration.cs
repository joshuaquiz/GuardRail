using System;
using System.Collections.Generic;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Keypad;

namespace GuardRail.Hardware.GuardRailCustom.Device.Configuration;

public sealed class KeypadConfiguration : IKeypadConfiguration<int>
{
    /// <inheritdoc />
    public TimeSpan KeypadTimeout { get; set; }

    /// <inheritdoc />
    public char SubmitKey { get; set; }

    /// <inheritdoc />
    public List<int> ColumnPins { get; set; } = new();

    /// <inheritdoc />
    public List<int> RowPins { get; set; } = new();
}