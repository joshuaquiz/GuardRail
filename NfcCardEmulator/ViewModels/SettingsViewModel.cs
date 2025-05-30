using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NfcCardEmulator.Models;
using NfcCardEmulator.Services;

namespace NfcCardEmulator.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SettingsViewModel> _logger;

    private string _apiEndpointUrl = "";
    private string _nfcApplicationId = "";
    private int _httpTimeoutSeconds = 10;
    private bool _hceServiceEnabled = true;

    public SettingsViewModel(ISettingsService settingsService, ILogger<SettingsViewModel> logger)
    {
        _settingsService = settingsService;
        _logger = logger;

        SaveCommand = new Command(async () => await SaveSettings());
        ResetCommand = new Command(async () => await ResetToDefaults());

        LoadSettings();
    }

    public string ApiEndpointUrl
    {
        get => _apiEndpointUrl;
        set => SetProperty(ref _apiEndpointUrl, value);
    }

    public string NfcApplicationId
    {
        get => _nfcApplicationId;
        set => SetProperty(ref _nfcApplicationId, value);
    }

    public int HttpTimeoutSeconds
    {
        get => _httpTimeoutSeconds;
        set => SetProperty(ref _httpTimeoutSeconds, value);
    }

    public bool HceServiceEnabled
    {
        get => _hceServiceEnabled;
        set => SetProperty(ref _hceServiceEnabled, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }

    private void LoadSettings()
    {
        try
        {
            var settings = _settingsService.GetSettings();
            
            ApiEndpointUrl = settings.ApiEndpointUrl;
            NfcApplicationId = settings.NfcApplicationId;
            HttpTimeoutSeconds = settings.HttpTimeoutSeconds;
            HceServiceEnabled = settings.HceServiceEnabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings");
        }
    }

    private async Task SaveSettings()
    {
        try
        {
            var settings = new AppSettings
            {
                ApiEndpointUrl = ApiEndpointUrl,
                NfcApplicationId = NfcApplicationId,
                HttpTimeoutSeconds = HttpTimeoutSeconds,
                HceServiceEnabled = HceServiceEnabled
            };

            await _settingsService.SaveSettingsAsync(settings);
            
            await Application.Current!.MainPage!.DisplayAlert("Success", "Settings saved successfully!", "OK");
            
            _logger.LogInformation("Settings saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings");
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
        }
    }

    private async Task ResetToDefaults()
    {
        try
        {
            var result = await Application.Current!.MainPage!.DisplayAlert(
                "Reset Settings", 
                "Are you sure you want to reset all settings to default values?", 
                "Yes", "No");

            if (result)
            {
                var defaultSettings = new AppSettings();
                
                ApiEndpointUrl = defaultSettings.ApiEndpointUrl;
                NfcApplicationId = defaultSettings.NfcApplicationId;
                HttpTimeoutSeconds = defaultSettings.HttpTimeoutSeconds;
                HceServiceEnabled = defaultSettings.HceServiceEnabled;

                await _settingsService.SaveSettingsAsync(defaultSettings);
                
                await Application.Current!.MainPage!.DisplayAlert("Success", "Settings reset to defaults!", "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting settings");
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to reset settings: {ex.Message}", "OK");
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
