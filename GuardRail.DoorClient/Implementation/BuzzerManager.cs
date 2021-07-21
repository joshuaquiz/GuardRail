using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;

namespace GuardRail.DoorClient.Implementation
{
    public sealed class BuzzerManager : IBuzzerManager, IDisposable
    {
        private readonly IGpio _gpio;
        private readonly BuzzerConfiguration _buzzerConfiguration;

        public BuzzerManager(IGpio gpio, BuzzerConfiguration buzzerConfiguration)
        {
            _gpio = gpio;
            _buzzerConfiguration = buzzerConfiguration;
            _gpio.OpenPin(_buzzerConfiguration.Pin, PinMode.Output);
        }

        public async Task Buzz(TimeSpan duration)
        {
            _gpio.Write(_buzzerConfiguration.Pin, PinValue.High);
            if (duration > TimeSpan.Zero)
            {
                await Task.Delay(duration);
                _gpio.Write(_buzzerConfiguration.Pin, PinValue.Low);
            }
        }

        public void Dispose()
        {
            _gpio.ClosePin(_buzzerConfiguration.Pin);
        }
    }
}