using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Logic
{
    public sealed class KeypadLogic : IKeypadLogic
    {
        private readonly ILightManager _lightManager;
        private readonly KeypadConfiguration _keypadConfiguration;
        private readonly IUdpSender _udpSender;
        private readonly ILogger<KeypadLogic> _logger;
        private readonly Timer _timer;

        private List<char> _pressedKeys;

        public KeypadLogic(
            ILightManager lightManager,
            KeypadConfiguration keypadConfiguration,
            IUdpSender udpSender,
            ILogger<KeypadLogic> logger)
        {
            _lightManager = lightManager;
            _keypadConfiguration = keypadConfiguration;
            _udpSender = udpSender;
            _logger = logger;
            _timer = new Timer(async _ => await Reset());
        }

        private async Task Reset()
        {
            _pressedKeys = new List<char>(0);
            await _lightManager.TurnOnRedLight(TimeSpan.FromMilliseconds(300));
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            await _lightManager.TurnOnRedLight(TimeSpan.FromMilliseconds(300));
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _logger.LogInformation("Keypad entry timed out.");
        }

        public Task OnKeyPressedAsync(char key, CancellationToken cancellationToken)
        {
            _timer.Change(TimeSpan.Zero, _keypadConfiguration.KeypadTimeout);
            if (key == _keypadConfiguration.SubmitKey)
            {
                _logger.LogInformation($"Key pressed: {key}");
                return _udpSender.SendUdpMessageAsync(
                    new UnLockRequest
                    {
                        UnlockRequestType = UnlockRequestType.Keypad,
                        Data = Encoding.UTF8.GetBytes(_pressedKeys.ToJson())
                    },
                    cancellationToken);
            }

            _pressedKeys.Add(key);
            _logger.LogInformation($"Key pressed: {key}");
            return Task.CompletedTask;
        }
    }
}