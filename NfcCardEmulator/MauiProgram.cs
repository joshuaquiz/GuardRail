using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using NfcCardEmulator.Services;
using NfcCardEmulator.ViewModels;
using NfcCardEmulator.Views;

namespace NfcCardEmulator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add services
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IBiometricService, BiometricService>();
        builder.Services.AddSingleton<INfcService, NfcService>();
        builder.Services.AddSingleton<HttpClient>();

        // Add view models
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Add views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SettingsPage>();

        // Add logging
        builder.Services.AddLogging(configure =>
        {
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Debug);
        });

        return builder.Build();
    }
}
