using NfcCardEmulator.Models;

namespace NfcCardEmulator.Services;

public interface ISettingsService
{
    AppSettings GetSettings();
    Task SaveSettingsAsync(AppSettings settings);
    Task<string> GetSettingAsync(string key, string defaultValue = "");
    Task SetSettingAsync(string key, string value);
}

public class SettingsService : ISettingsService
{
    private const string SettingsKey = "app_settings";
    private AppSettings? _cachedSettings;

    public AppSettings GetSettings()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        var settingsJson = Preferences.Get(SettingsKey, string.Empty);
        
        if (string.IsNullOrEmpty(settingsJson))
        {
            _cachedSettings = new AppSettings();
            return _cachedSettings;
        }

        try
        {
            _cachedSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(settingsJson) ?? new AppSettings();
        }
        catch
        {
            _cachedSettings = new AppSettings();
        }

        return _cachedSettings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        _cachedSettings = settings;
        var settingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
        Preferences.Set(SettingsKey, settingsJson);
        await Task.CompletedTask;
    }

    public async Task<string> GetSettingAsync(string key, string defaultValue = "")
    {
        return await Task.FromResult(Preferences.Get(key, defaultValue));
    }

    public async Task SetSettingAsync(string key, string value)
    {
        Preferences.Set(key, value);
        await Task.CompletedTask;
    }
}
