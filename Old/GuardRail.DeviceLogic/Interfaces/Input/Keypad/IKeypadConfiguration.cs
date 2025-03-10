using System;
using System.Collections.Generic;

namespace GuardRail.DeviceLogic.Interfaces.Input.Keypad;

/// <summary>
/// Base configuration for a keypad.
/// </summary>
/// <typeparam name="T">The type used for the keypad's addresses.</typeparam>
public interface IKeypadConfiguration<T>
{
    /// <summary>
    /// The length of time to wait until previous input in invalidated.
    /// </summary>
    /// <remarks>
    /// If set to 3 sec. and a user inputs "12" then pauses > 3 sec. then inputs "3" the "12" will no longer be stored.
    /// </remarks>
    public TimeSpan KeypadTimeout { get; set; }

    /// <summary>
    /// The key that will act as the submit key.
    /// </summary>
    /// <remarks>
    /// This key will not record its value but will trigger a submit.
    /// </remarks>
    public char SubmitKey { get; set; }

    /// <summary>
    /// The addresses of the columns.
    /// </summary>
    public List<T> ColumnPins { get; set; }

    /// <summary>
    /// The addresses of the rows.
    /// </summary>
    public List<T> RowPins { get; set; }
}