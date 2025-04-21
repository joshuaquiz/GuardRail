using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Configuration;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Input;

public sealed class KeypadHardwareManager(
    IGpio gpio,
    KeypadConfiguration keypadConfiguration,
    ILogger<KeypadHardwareManager> logger)
    : IKeypadHardwareManager<int>
{
    private const int ButtonDebounceMilliseconds = 25;

    private readonly char[,] _keypad = {
        {'1', '2', '3', 'A'},
        {'4', '5', '6', 'B'},
        {'7', '8', '9', 'C'},
        {'*', '0', '#', 'D'}
    };

    public readonly List<int> Rows = new();
    public readonly List<int> Columns = new();

    private CancellationTokenSource _cancellationTokenSource = new();
    private List<char> _pressedKeys = new(0);
    private string _lastPinValue = string.Empty;
    private DateTime _clearLastPinValueTime = DateTime.UtcNow;

    public ValueTask InitAsync()
    {
        // Initialize Column GPIO Pins
        gpio.OpenPin(keypadConfiguration.ColumnPins[0], PinMode.Output);
        gpio.Write(keypadConfiguration.ColumnPins[0], PinValue.Low);
        gpio.OpenPin(keypadConfiguration.ColumnPins[1], PinMode.Output);
        gpio.Write(keypadConfiguration.ColumnPins[1], PinValue.Low);
        gpio.OpenPin(keypadConfiguration.ColumnPins[2], PinMode.Output);
        gpio.Write(keypadConfiguration.ColumnPins[2], PinValue.Low);
        gpio.OpenPin(keypadConfiguration.ColumnPins[3], PinMode.Output);
        gpio.Write(keypadConfiguration.ColumnPins[3], PinValue.Low);

        // Add to Cols Array
        Columns.Add(keypadConfiguration.ColumnPins[0]);
        Columns.Add(keypadConfiguration.ColumnPins[1]);
        Columns.Add(keypadConfiguration.ColumnPins[2]);
        Columns.Add(keypadConfiguration.ColumnPins[3]);

        // Initialize Row GPIO Pins
        gpio.OpenPin(keypadConfiguration.RowPins[0]);
        gpio.RegisterCallbackForPinValueChangedEvent(keypadConfiguration.RowPins[0], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber).GetAwaiter().GetResult());
        gpio.OpenPin(keypadConfiguration.RowPins[1]);
        gpio.RegisterCallbackForPinValueChangedEvent(keypadConfiguration.RowPins[1], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber).GetAwaiter().GetResult());
        gpio.OpenPin(keypadConfiguration.RowPins[2]);
        gpio.RegisterCallbackForPinValueChangedEvent(keypadConfiguration.RowPins[2], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber).GetAwaiter().GetResult());
        gpio.OpenPin(keypadConfiguration.RowPins[3]);
        gpio.RegisterCallbackForPinValueChangedEvent(keypadConfiguration.RowPins[3], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber).GetAwaiter().GetResult());

        // Add to Row Array
        Rows.Add(keypadConfiguration.RowPins[0]);
        Rows.Add(keypadConfiguration.RowPins[1]);
        Rows.Add(keypadConfiguration.RowPins[2]);
        Rows.Add(keypadConfiguration.RowPins[3]);

        // Set the rows up for input
        foreach (var r in Rows)
        {
            gpio.SetPinMode(r, PinMode.InputPullUp);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public event Func<string, CancellationToken, ValueTask>? Submit;

    /// <inheritdoc />
    public event Func<CancellationToken, ValueTask>? Reset;

    public ValueTask DisposeAddressAsync(
        int address)
    {
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }

    private async Task Pin_Changed(int pinNumber)
    {
        var colNumber = -1;
        var rowNumber = -1;
        try
        {
            var senderValue = gpio.Read(pinNumber);
            if (senderValue.ToString() == _lastPinValue)
            {
                // Skipping duplicate value for pin.
                return;
            }

            // Key was pressed
            if (senderValue != PinValue.High)
            {
                return;
            }

            // store last pressed value to help debounce value
            _lastPinValue = senderValue.ToString();
            _clearLastPinValueTime = DateTime.UtcNow.AddMilliseconds(ButtonDebounceMilliseconds);

            // Get the corresponding row index for conversion later
            if (pinNumber == keypadConfiguration.RowPins[0])
            {
                rowNumber = 0;
            }
            else if (pinNumber == keypadConfiguration.RowPins[1])
            {
                rowNumber = 1;
            }
            else if (pinNumber == keypadConfiguration.RowPins[2])
            {
                rowNumber = 2;
            }
            else if (pinNumber == keypadConfiguration.RowPins[3])
            {
                rowNumber = 3;
            }

            // Set all the columns to low value using output mode
            foreach (var c in Columns)
            {
                gpio.SetPinMode(c, PinMode.Output);
                gpio.Write(c, PinValue.Low);
            }

            // Now switch the columns to input mode
            foreach (var c in Columns)
            {
                gpio.SetPinMode(c, PinMode.InputPullDown);
            }

            // Scan the columns to see which one is pressed
            foreach (var c in Columns.Where(c => gpio.Read(c) == PinValue.High))
            {
                // Get the corresponding column index for conversion later
                if (c == keypadConfiguration.ColumnPins[0])
                {
                    colNumber = 0;
                }
                else if (c == keypadConfiguration.ColumnPins[1])
                {
                    colNumber = 1;
                }
                else if (c == keypadConfiguration.ColumnPins[2])
                {
                    colNumber = 2;
                }
                else if (c == keypadConfiguration.ColumnPins[3])
                {
                    colNumber = 3;
                }
            }

            // If both a row and column were found, then we have a valid input!
            if (colNumber < 0 || rowNumber < 0)
            {
                return;
            }

            await FoundADigit(_keypad[colNumber, rowNumber]);
        }
        catch (Exception ex)
        {
            logger.LogGuardRailError(ex, $"Pin_Changed Error: {pinNumber} {rowNumber} {colNumber} {ex.Message}");
        }
    }

    /// <summary>
    /// Sends message to subscribers telling them a digit was pressed.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    private async Task FoundADigit(char key)
    {
        _cancellationTokenSource.Cancel();
        logger.LogGuardRailInformation($"Key pressed: {key}");
        if (key == keypadConfiguration.SubmitKey)
        {
            var keyData = _pressedKeys.ToList();
            _pressedKeys = new List<char>(0);
            logger.LogGuardRailInformation($"Sending keys: {string.Join(", ", keyData)}");
            var cancellationTokenSource = new CancellationTokenSource(keypadConfiguration.KeypadTimeout * 2);
            if (Submit != null)
            {
                await Submit(
                    new string(keyData.ToArray()),
                    cancellationTokenSource.Token);
            }
        }

        _pressedKeys.Add(key);
        logger.LogGuardRailInformation($"Keys pressed so far: {string.Join(", ", _pressedKeys)}");
        _cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable 4014
        // We want this to be fire-and-forget
        DelayThenReset();
#pragma warning restore 4014
    }

    /// <inheritdoc />
    public void TimerTick()
    {
        // If it's been a while, then clear the last value so you enter another one.
        if (_clearLastPinValueTime > DateTime.UtcNow)
        {
            return;
        }

        _lastPinValue = string.Empty;
        // Reset the next check a ways out in the future.
        _clearLastPinValueTime = DateTime.UtcNow.AddSeconds(15);
    }

    private async Task DelayThenReset()
    {
        try
        {
            await Task.Delay(keypadConfiguration.KeypadTimeout, _cancellationTokenSource.Token);
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            logger.LogGuardRailInformation("Keypad entry timed out.");
            _pressedKeys = new List<char>(0);
            if (Reset != null)
            {
                await Reset(_cancellationTokenSource.Token);
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogGuardRailInformation("Keypad entry time out canceled.");
        }
    }
}