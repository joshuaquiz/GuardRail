using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NfcCardEmulator.Models;
using NfcCardEmulator.Services;

namespace NfcCardEmulator.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly INfcService _nfcService;
    private readonly IBiometricService _biometricService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<MainViewModel> _logger;

    private string _statusMessage = "Initializing...";
    private string _statusColor = "Gray";
    private bool _isHceEnabled = true;
    private bool _isNfcAvailable;
    private bool _isBiometricAvailable;

    public MainViewModel(
        INfcService nfcService,
        IBiometricService biometricService,
        ISettingsService settingsService,
        ILogger<MainViewModel> logger)
    {
        _nfcService = nfcService;
        _biometricService = biometricService;
        _settingsService = settingsService;
        _logger = logger;

        _nfcService.StatusChanged += OnNfcStatusChanged;

        ToggleHceCommand = new Command(ToggleHce);
        TestBiometricCommand = new Command(async () => await TestBiometric());
        OpenSettingsCommand = new Command(async () => await OpenSettings());

        _ = InitializeAsync();
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string StatusColor
    {
        get => _statusColor;
        set => SetProperty(ref _statusColor, value);
    }

    public bool IsHceEnabled
    {
        get => _isHceEnabled;
        set => SetProperty(ref _isHceEnabled, value);
    }

    public bool IsNfcAvailable
    {
        get => _isNfcAvailable;
        set => SetProperty(ref _isNfcAvailable, value);
    }

    public bool IsBiometricAvailable
    {
        get => _isBiometricAvailable;
        set => SetProperty(ref _isBiometricAvailable, value);
    }

    public ICommand ToggleHceCommand { get; }
    public ICommand TestBiometricCommand { get; }
    public ICommand OpenSettingsCommand { get; }

    private async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing MainViewModel");

            IsNfcAvailable = await _nfcService.IsNfcAvailableAsync();
            IsBiometricAvailable = await _biometricService.IsAvailableAsync();

            var settings = _settingsService.GetSettings();
            IsHceEnabled = settings.HceServiceEnabled;

            if (!IsNfcAvailable)
            {
                StatusMessage = "NFC not available on this device";
                StatusColor = "Red";
            }
            else if (!IsBiometricAvailable)
            {
                StatusMessage = "Biometric authentication not available";
                StatusColor = "Orange";
            }
            else if (IsHceEnabled)
            {
                _nfcService.EnableHceService();
            }
            else
            {
                StatusMessage = "NFC HCE Service Disabled";
                StatusColor = "Gray";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing MainViewModel");
            StatusMessage = $"Initialization error: {ex.Message}";
            StatusColor = "Red";
        }
    }

    private void OnNfcStatusChanged(object? sender, NfcStatusEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusMessage = e.Message;
            StatusColor = e.Status switch
            {
                NfcStatus.Ready => "Green",
                NfcStatus.AwaitingFingerprint => "Orange",
                NfcStatus.FetchingData => "Blue",
                NfcStatus.Transmitting => "Purple",
                NfcStatus.Success => "Green",
                NfcStatus.Error => "Red",
                NfcStatus.Disabled => "Gray",
                _ => "Gray"
            };
        });
    }

    private void ToggleHce()
    {
        try
        {
            IsHceEnabled = !IsHceEnabled;
            
            var settings = _settingsService.GetSettings();
            settings.HceServiceEnabled = IsHceEnabled;
            _ = _settingsService.SaveSettingsAsync(settings);

            if (IsHceEnabled)
            {
                _nfcService.EnableHceService();
            }
            else
            {
                _nfcService.DisableHceService();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling HCE service");
        }
    }

    private async Task TestBiometric()
    {
        try
        {
            var result = await _biometricService.AuthenticateAsync("Test biometric authentication");
            
            if (result)
            {
                await Application.Current!.MainPage!.DisplayAlert("Success", "Biometric authentication successful!", "OK");
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Failed", "Biometric authentication failed!", "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing biometric authentication");
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async Task OpenSettings()
    {
        try
        {
            // Navigate to settings page
            await Shell.Current.GoToAsync("//settings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening settings");
        }
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
