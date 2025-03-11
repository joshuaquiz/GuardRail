using System;
using System.Net.Http;
using System.Windows;

namespace GuardRail.Local.Updater;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static HttpClient? _httpClient;

    /// <summary>
    /// Initial app startup configuration.
    /// </summary>
    public App()
    {
        _httpClient = new HttpClient();
#if DEBUG
        _httpClient.BaseAddress = new Uri("https://localhost:5050/");
#else
        HttpClient.BaseAddress = new Uri("https://url.url:5050/");
#endif
        Startup += Application_Startup;
    }

    private static void Application_Startup(
        object sender,
        StartupEventArgs e)
    {
        new MainWindow(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                _httpClient!)
            .Show();
    }
}