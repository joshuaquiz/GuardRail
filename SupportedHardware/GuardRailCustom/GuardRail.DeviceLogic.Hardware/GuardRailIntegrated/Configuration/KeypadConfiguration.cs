using System;
using System.Collections.Generic;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;

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