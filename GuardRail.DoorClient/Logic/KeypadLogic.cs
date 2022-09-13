using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Logic;

public sealed class KeypadLogic : IKeypadLogic
{
    private readonly ILightManager _lightManager;
    private readonly KeypadConfiguration _keypadConfiguration;
    private readonly IUdpSenderReceiver _udpSenderReceiver;
    private readonly ILogger<KeypadLogic> _logger;

    private CancellationTokenSource _cancellationTokenSource;

    private List<char> _pressedKeys = new(0);

    public KeypadLogic(
        ILightManager lightManager,
        KeypadConfiguration keypadConfiguration,
        IUdpSenderReceiver udpSenderReceiver,
        ILogger<KeypadLogic> logger)
    {
        _lightManager = lightManager;
        _keypadConfiguration = keypadConfiguration;
        _udpSenderReceiver = udpSenderReceiver;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private async Task DelayThenReset()
    {
        try
        {
            await Task.Delay(_keypadConfiguration.KeypadTimeout, _cancellationTokenSource.Token);
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            LogInformation("Keypad entry timed out.");
            _pressedKeys = new List<char>(0);
            await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), _cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationTokenSource.Token);
            await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), _cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            LogInformation("Keypad entry time out canceled.");
            // Ignored.
        }
    }

    public Task OnKeyPressedAsync(char key)
    {
        _cancellationTokenSource.Cancel();
        LogInformation($"Key pressed: {key}");
        if (key == _keypadConfiguration.SubmitKey)
        {
            var keyData = _pressedKeys.ToList();
            _pressedKeys = new List<char>(0);
            LogInformation($"Sending keys: {string.Join(", ", keyData)}");
            var cancellationTokenSource = new CancellationTokenSource(_keypadConfiguration.KeypadTimeout * 2);
            return _udpSenderReceiver.SendUdpMessageAsync(
                new UnLockRequest
                {
                    UnlockRequestType = UnlockRequestType.Keypad,
                    Data = Encoding.UTF8.GetBytes(keyData.ToJson())
                },
                cancellationTokenSource.Token);
        }

        _pressedKeys.Add(key);
        LogInformation($"Keys pressed so far: {string.Join(", ", _pressedKeys)}");
        _cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable 4014
        // We want this to be fire-and-forget
        DelayThenReset();
#pragma warning restore 4014
        return Task.CompletedTask;
    }

    private void LogInformation(string message) =>
        _logger.LogInformation("[keypad logic] " + message);
}