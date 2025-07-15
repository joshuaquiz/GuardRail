using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NfcCardEmulator.Models;

namespace NfcCardEmulator.Services;

public interface INfcService
{
    event EventHandler<NfcStatusEventArgs>? StatusChanged;
    Task<bool> IsNfcAvailableAsync();
    Task<bool> IsHceAvailableAsync();
    void EnableHceService();
    void DisableHceService();
    NfcStatus CurrentStatus { get; }
}

public class NfcService : INfcService
{
    public event EventHandler<NfcStatusEventArgs>? StatusChanged;
    public NfcStatus CurrentStatus { get; private set; } = NfcStatus.Ready;

    private readonly ILogger<NfcService> _logger;

    public NfcService(ILogger<NfcService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsNfcAvailableAsync()
    {
#if ANDROID
        try
        {
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking NFC availability");
            return false;
        }
#else
        // iOS does not support HCE for general purpose card emulation
        await Task.CompletedTask;
        return false;
#endif
    }

    public async Task<bool> IsHceAvailableAsync()
    {
#if ANDROID
        try
        {
            var context = Platform.CurrentActivity ?? Android.App.Application.Context;
            var packageManager = context.PackageManager;
            return await Task.FromResult(
                packageManager?.HasSystemFeature(Android.Content.PM.PackageManager.FeatureNfcHostCardEmulation) == true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking HCE availability");
            return false;
        }
#else
        await Task.CompletedTask;
        return false;
#endif
    }

    public void EnableHceService()
    {
        try
        {
            UpdateStatus(NfcStatus.Ready, "NFC HCE Service Enabled");
            _logger.LogInformation("HCE Service enabled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling HCE service");
            UpdateStatus(NfcStatus.Error, "Failed to enable HCE service");
        }
    }

    public void DisableHceService()
    {
        try
        {
            UpdateStatus(NfcStatus.Disabled, "NFC HCE Service Disabled");
            _logger.LogInformation("HCE Service disabled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling HCE service");
        }
    }

    internal void UpdateStatus(NfcStatus status, string message, Exception? exception = null)
    {
        CurrentStatus = status;
        StatusChanged?.Invoke(this, new NfcStatusEventArgs 
        { 
            Status = status, 
            Message = message, 
            Exception = exception 
        });
    }
}
