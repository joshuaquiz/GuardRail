using System;
using System.Net.Http;
using System.Windows;

namespace GuardRail.Local.Updater;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly HttpClient HttpClient;

    static App()
    {
        HttpClient = new HttpClient();
#if DEBUG
        HttpClient.BaseAddress = new Uri("https://localhost:5050/");
#else
        HttpClient.BaseAddress = new Uri("https://url.url:5050/");
#endif
    }

    /// <summary>
    /// Initial app startup configuration.
    /// </summary>
    public App()
    {
        Startup += Application_Startup;
    }

    private static void Application_Startup(
        object sender,
        StartupEventArgs e)
    {
        new MainWindow(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                HttpClient)
            .Show();
    }
}