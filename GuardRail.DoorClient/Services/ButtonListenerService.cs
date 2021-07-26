using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Exceptions;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services
{
    public sealed class ButtonListenerService : IHostedService
    {
        private const int TimerIntervalMilliseconds = 20;
        private const int ButtonDebounceMilliseconds = 25;

        private string SetupMessage { get; set; }

        private readonly IGpio _gpio;
        private readonly KeypadConfiguration _keypadConfiguration;
        private readonly IKeypadLogic _keypadLogic;
        private readonly ILogger<ButtonListenerService> _logger;
        private readonly List<int> _rows = new();
        private readonly List<int> _cols = new();
        private readonly char[,] _keypad = {
            {'1', '2', '3', 'A'},
            {'4', '5', '6', 'B'},
            {'7', '8', '9', 'C'},
            {'*', '0', '#', 'D'}
        };

        private Timer _dispatcherTimer;
        private string _lastPinValue = string.Empty;
        private DateTime _clearLastPinValueTime = DateTime.UtcNow;

        public ButtonListenerService(
            IGpio gpio,
            KeypadConfiguration keypadConfiguration,
            IKeypadLogic keypadLogic,
            ILogger<ButtonListenerService> logger)
        {
            _gpio = gpio;
            _keypadConfiguration = keypadConfiguration;
            _keypadLogic = keypadLogic;
            _logger = logger;
            // validate inputs
            if (keypadConfiguration.ColumnPins.Count == 4
                && keypadConfiguration.RowPins.Count == 4)
            {
                return;
            }

            SetupMessage = "Please supply a list of 4 GPIO column and 4 GPIO row pin numbers.";
            _logger.LogCritical(SetupMessage);
            throw new InvalidPinSetupException(SetupMessage);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (InitializeGpio())
            {
                _logger.LogInformation("GPIO setup for button listener");
                _dispatcherTimer = new Timer(
                    _ => TimerTick(),
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(TimerIntervalMilliseconds));
                return Task.CompletedTask;
            }

            SetupMessage = $"GPIO Initialization failed! {SetupMessage}";
            _logger.LogCritical(SetupMessage);
            return Task.FromException(new GpioSetupException(SetupMessage));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _dispatcherTimer.DisposeAsync();
            foreach (var pin in _keypadConfiguration.ColumnPins)
            {
                _gpio.ClosePin(pin);
            }

            foreach (var pin in _keypadConfiguration.RowPins)
            {
                _gpio.ClosePin(pin);
            }
        }

        private bool InitializeGpio()
        {
            if (_gpio != null)
            {
                // Initialize Column GPIO Pins
                _gpio.OpenPin(_keypadConfiguration.ColumnPins[0], PinMode.Output);
                _gpio.Write(_keypadConfiguration.ColumnPins[0], PinValue.Low);
                _gpio.OpenPin(_keypadConfiguration.ColumnPins[1], PinMode.Output);
                _gpio.Write(_keypadConfiguration.ColumnPins[1], PinValue.Low);
                _gpio.OpenPin(_keypadConfiguration.ColumnPins[2], PinMode.Output);
                _gpio.Write(_keypadConfiguration.ColumnPins[2], PinValue.Low);
                _gpio.OpenPin(_keypadConfiguration.ColumnPins[3], PinMode.Output);
                _gpio.Write(_keypadConfiguration.ColumnPins[3], PinValue.Low);

                // Add to Cols Array
                _cols.Add(_keypadConfiguration.ColumnPins[0]);
                _cols.Add(_keypadConfiguration.ColumnPins[1]);
                _cols.Add(_keypadConfiguration.ColumnPins[2]);
                _cols.Add(_keypadConfiguration.ColumnPins[3]);

                // Initialize Row GPIO Pins
                _gpio.OpenPin(_keypadConfiguration.RowPins[0]);
                _gpio.RegisterCallbackForPinValueChangedEvent(_keypadConfiguration.RowPins[0], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber));
                _gpio.OpenPin(_keypadConfiguration.RowPins[1]);
                _gpio.RegisterCallbackForPinValueChangedEvent(_keypadConfiguration.RowPins[1], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber));
                _gpio.OpenPin(_keypadConfiguration.RowPins[2]);
                _gpio.RegisterCallbackForPinValueChangedEvent(_keypadConfiguration.RowPins[2], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber));
                _gpio.OpenPin(_keypadConfiguration.RowPins[3]);
                _gpio.RegisterCallbackForPinValueChangedEvent(_keypadConfiguration.RowPins[3], PinEventTypes.Falling | PinEventTypes.Rising, (_, args) => Pin_Changed(args.PinNumber));

                // Add to Row Array
                _rows.Add(_keypadConfiguration.RowPins[0]);
                _rows.Add(_keypadConfiguration.RowPins[1]);
                _rows.Add(_keypadConfiguration.RowPins[2]);
                _rows.Add(_keypadConfiguration.RowPins[3]);

                // Set the rows up for input
                foreach (var r in _rows)
                {
                    _gpio.SetPinMode(r, PinMode.InputPullUp);
                }

                return true;
            }

            SetupMessage = "GPIO Controller not found!";
            throw new GpioSetupException(SetupMessage);
        }

        private void TimerTick()
        {
            // if it's been a while, then clear the last value so you enter another one
            if (_clearLastPinValueTime > DateTime.UtcNow)
            {
                return;
            }

            _lastPinValue = string.Empty;
            // reset the next check a ways out in the future
            _clearLastPinValueTime = DateTime.UtcNow.AddSeconds(15);
        }

        private void Pin_Changed(int pinNumber)
        {
            var colNumber = -1;
            var rowNumber = -1;
            try
            {
                var senderValue = _gpio.Read(pinNumber);
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
                if (pinNumber == _keypadConfiguration.RowPins[0])
                {
                    rowNumber = 0;
                }
                else if(pinNumber == _keypadConfiguration.RowPins[1])
                {
                    rowNumber = 1;
                }
                else if (pinNumber == _keypadConfiguration.RowPins[2])
                {
                    rowNumber = 2;
                }
                else if (pinNumber == _keypadConfiguration.RowPins[3])
                {
                    rowNumber = 3;
                }

                // Set all the columns to low value using output mode
                foreach (var c in _cols)
                {
                    _gpio.SetPinMode(c, PinMode.Output);
                    _gpio.Write(c, PinValue.Low);
                }

                // Now switch the columns to input mode
                foreach (var c in _cols)
                {
                    _gpio.SetPinMode(c, PinMode.InputPullDown);
                }

                // Scan the columns to see which one is pressed
                foreach (var c in _cols.Where(c => _gpio.Read(c) == PinValue.High))
                {
                    // Get the corresponding column index for conversion later
                    if (c == _keypadConfiguration.ColumnPins[0])
                    {
                        colNumber = 0;
                    }
                    else if(c == _keypadConfiguration.ColumnPins[1])
                    {
                        colNumber = 1;
                    }
                    else if (c == _keypadConfiguration.ColumnPins[2])
                    {
                        colNumber = 2;
                    }
                    else if (c == _keypadConfiguration.ColumnPins[3])
                    {
                        colNumber = 3;
                    }
                }

                // If both a row and column were found, then we have a valid input!
                if (colNumber < 0 || rowNumber < 0)
                {
                    return;
                }
                    
                FoundADigit(_keypad[colNumber, rowNumber]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Pin_Changed Error: {pinNumber} {rowNumber} {colNumber} {ex.Message}");
            }
        }

        /// <summary>
        /// Sends message to subscribers telling them a digit was pressed.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <param name="cancellationToken"></param>
        private void FoundADigit(char key)
        {
            _logger.LogInformation($"Found character {key}");
            _keypadLogic.OnKeyPressedAsync(key);
        }
    }
}