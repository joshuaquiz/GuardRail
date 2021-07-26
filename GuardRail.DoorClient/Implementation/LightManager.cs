using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation
{
    public sealed class LightManager : ILightManager, IDisposable
    {
        private readonly IGpio _gpio;
        private readonly LightConfiguration _lightConfiguration;
        private readonly ILogger<LightManager> _logger;

        public LightManager(IGpio gpio, LightConfiguration lightConfiguration, ILogger<LightManager> logger)
        {
            _gpio = gpio;
            _lightConfiguration = lightConfiguration;
            _logger = logger;
            _logger.LogInformation($"Opening the pins for the lights: r:{_lightConfiguration.RedPin} g:{_lightConfiguration.GreenPin}");
            _gpio.OpenPin(_lightConfiguration.RedPin, PinMode.Output);
            _gpio.OpenPin(_lightConfiguration.GreenPin, PinMode.Output);
        }

        public async Task TurnOnRedLightAsync(TimeSpan duration, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Turning red light on");
            _gpio.Write(_lightConfiguration.RedPin, PinValue.High);
            if (duration > TimeSpan.Zero)
            {
                await Task.Delay(duration, cancellationToken);
                _logger.LogDebug("Turning red light off");
                _gpio.Write(_lightConfiguration.RedPin, PinValue.Low);
            }
        }

        public async Task TurnOnGreenLightAsync(TimeSpan duration, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Turning green light on");
            _gpio.Write(_lightConfiguration.GreenPin, PinValue.High);
            if (duration > TimeSpan.Zero)
            {
                await Task.Delay(duration, cancellationToken);
                _logger.LogDebug("Turning green light off");
                _gpio.Write(_lightConfiguration.GreenPin, PinValue.Low);
            }
        }

        public void Dispose()
        {
            _gpio.ClosePin(_lightConfiguration.RedPin);
            _gpio.ClosePin(_lightConfiguration.GreenPin);
        }
    }
}