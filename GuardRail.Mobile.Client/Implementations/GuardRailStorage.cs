using System.Text.Json;
using System.Threading.Tasks;
using GuardRail.ApiModels.Responses;
using GuardRail.Mobile.Client.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace GuardRail.Mobile.Client.Implementations;

public sealed class GuardRailStorage(
        ISecureStorage secureStorage,
        IPreferences preferences)
    : IGuardRailStorage
{
    public Task<AppTheme> GetCurrentAppTheme(
        Application application)
    {
        var value = preferences.Get(
            nameof(AppTheme),
            (int) application.UserAppTheme);
        return Task.FromResult(
            (AppTheme) value);
    }

    public Task SetAppTheme(
        AppTheme appTheme,
        Application application)
    {
        preferences.Set(
            nameof(AppTheme),
            (int) appTheme);
        application.UserAppTheme = appTheme;
        return Task.CompletedTask;
    }

    public Task Empty()
    {
        preferences.Clear();
        secureStorage.RemoveAll();
        return Task.CompletedTask;
    }

    public async Task SetUser(
        MobileLoginResponse user) =>
        await secureStorage.SetAsync(
            "UserProfile",
            JsonSerializer.Serialize(
                user));

    public MobileLoginResponse? GetUser()
    {
        var profileAsString = Task.Run(async () => await secureStorage.GetAsync("UserProfile")).Result;
        return profileAsString == null
            ? null
            : JsonSerializer.Deserialize<MobileLoginResponse>(
                profileAsString);
    }
}