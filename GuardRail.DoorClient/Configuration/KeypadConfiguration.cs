using System;
using System.Collections.Generic;

namespace GuardRail.DoorClient.Configuration;

public sealed class KeypadConfiguration
{
    public List<int> ColumnPins { get; set; }

    public List<int> RowPins { get; set; }

    public TimeSpan KeypadTimeout { get; set; }

    public char SubmitKey { get; set; }
}