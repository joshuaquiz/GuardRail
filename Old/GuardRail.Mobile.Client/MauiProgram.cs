using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using System.Net.Http;
using GuardRail.Mobile.Client.Implementations;
using GuardRail.Mobile.Client.Interfaces;
using GuardRail.Mobile.Client.ViewModels;
using GuardRail.Mobile.Client.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;
using Plugin.NFC;
using Plugin.Fingerprint;

namespace GuardRail.Mobile.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if DEBUG
        builder.Services
            .AddSingleton(
                serviceProvider =>
                    new FakedDataDelegatingHandler(
                        serviceProvider.GetRequiredService<GuardRailDelegatingHandler>()))
            .AddSingleton(
                serviceProvider =>
                {
                    var handler = serviceProvider.GetRequiredService<FakedDataDelegatingHandler>();
                    return new HttpClient(handler);
                });
#else
        builder.Services
            .AddSingleton(
                serviceProvider =>
                {
                    var handler = serviceProvider.GetRequiredService<GuardRailDelegatingHandler>();
                    return new HttpClient(handler);
                });
#endif
        builder.Services
            .AddMemoryCache()
            .AddSingleton(SecureStorage.Default)
            .AddSingleton(Preferences.Default)
            .AddSingleton(Connectivity.Current)
            .AddSingleton(Geolocation.Default)
            .AddSingleton(CrossNFC.Current)
            .AddSingleton(CrossFingerprint.Current);

        // Event raised when a ndef message is received.
        CrossNFC.Current.OnMessageReceived += NfcHandler.Current_OnMessageReceived;
        // Event raised when a ndef message has been published.
        CrossNFC.Current.OnMessagePublished += NfcHandler.Current_OnMessagePublished;
        // Event raised when a tag is discovered. Used for publishing.
        CrossNFC.Current.OnTagDiscovered += NfcHandler.Current_OnTagDiscovered;
        // Event raised when NFC listener status changed
        CrossNFC.Current.OnTagListeningStatusChanged += NfcHandler.Current_OnTagListeningStatusChanged;

        // Android Only:
        // Event raised when NFC state has changed.
        CrossNFC.Current.OnNfcStatusChanged += NfcHandler.Current_OnNfcStatusChanged;

        // iOS Only:
        // Event raised when a user cancelled NFC session.
        CrossNFC.Current.OniOSReadingSessionCancelled += NfcHandler.Current_OniOSReadingSessionCancelled;

        builder.Services
            .AddPlatformSpecificImplementations()
            .AddSingleton(
                serviceProvider =>
                    new GuardRailDelegatingHandler(
                        new HttpClientHandler(),
                        serviceProvider.GetRequiredService<IGuardRailStorage>()))
            .AddSingleton<IGuardRailStorage, GuardRailStorage>()
            .AddSingleton<IGuardRailHttpClient, GuardRailHttpClient>()
            .AddSingleton<INfcHandler, NfcHandler>();

        builder.Services
            .AddSingleton<LoginViewModel>()
            .AddSingleton<HomePageViewModel>();

        builder.Services
            .AddSingleton<Login>()
            .AddSingleton<HomePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}