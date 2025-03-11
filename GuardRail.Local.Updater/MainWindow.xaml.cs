using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GuardRail.Api.Models.Responses;
using GuardRail.Core.Helpers;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace GuardRail.Local.Updater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Version _version;
    private readonly string _applicationRootFolder;
    private readonly HttpClient _httpClient;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private VersionCheckResponse? _installConfiguration;

    /// <summary>
    /// Setup window.
    /// </summary>
    public MainWindow(
        string applicationRootFolder,
        HttpClient httpClient)
    {
        _version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 1);
        _applicationRootFolder = applicationRootFolder;
        _httpClient = httpClient;
        InitializeComponent();
        _cancellationTokenSource = new CancellationTokenSource();
        Status.Content = "Checking for updates...";
        Loaded += OnLoaded;
        Unloaded += (_, _) => _cancellationTokenSource.Cancel();
    }

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (await HasUpdate())
        {
            await DownloadUpdate();
            StartUpdate();
        }
        else
        {
            Status.Content = "GuardRail is already up to date!";
            ProgressBar.Value = 100;
        }
    }

    private async Task<bool> HasUpdate()
    {
        Status.Content = "Checking version...";
        var uriString = Application.Current.TryFindResource("UpdateUri") as string;
        if (uriString.IsNullOrWhiteSpace())
        {
            throw new ConfigurationErrorsException("Error: No download URL");
        }

        _installConfiguration = await _httpClient
            .GetFromJsonAsync<VersionCheckResponse>(
                $"/VersionCheck?version={_version}");
        return _installConfiguration?.IsLatest == true;
    }

    private async Task DownloadUpdate()
    {
        Status.Content = "Downloading...";
        var latestVersionFolder = _applicationRootFolder + _installConfiguration!.LatestVersion;
        if (!Directory.Exists(latestVersionFolder))
        {
            Directory.Delete(latestVersionFolder, true);
            Directory.CreateDirectory(latestVersionFolder);
        }

        ProgressBar.Maximum = _installConfiguration!.InstallFiles?.Count ?? 0;
        await Task.WhenAll(_installConfiguration.InstallFiles?.Select(SaveFile) ?? []);
        Status.Content = "Downloading complete";
    }

    private async Task SaveFile(
        KeyValuePair<string, string> installFile)
    {
        var filePath = _applicationRootFolder + _version + installFile.Key;
        await File.WriteAllBytesAsync(
            filePath,
            await _httpClient
                .GetByteArrayAsync(
                    installFile.Value),
            _cancellationTokenSource.Token);
        if (filePath.EndsWith(".zip"))
        {
            ZipFile.ExtractToDirectory(
                filePath,
                _applicationRootFolder + _version);
            File.Delete(filePath);
        }

        ProgressBar.Value++;
    }

    private void StartUpdate()
    {
        ProgressBar.Value = 0;
        Status.Content = "Stopping GuardRail...";
        var processes = Process.GetProcessesByName("GuardRail");
        foreach (var process in processes)
        {
            if (!process.CloseMainWindow())
            {
                process.Kill();
            }
        }

        Status.Content = "Updating Installation Files...";
        Directory.Move(
            Path.Combine(_applicationRootFolder, "Latest"),
            Path.Combine(_applicationRootFolder, _version.ToString()));
        Directory.Move(
            Path.Combine(_applicationRootFolder, _installConfiguration!.LatestVersion!.ToString()),
            Path.Combine(_applicationRootFolder, "Latest"));
        Status.Content = "Creating Shortcuts...";
        CreateShortcut(Environment.SpecialFolder.Desktop);
        CreateShortcut(Environment.SpecialFolder.CommonStartup);
        var commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        var appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "GuardRail");
        if (!Directory.Exists(appStartMenuPath))
        {
            Directory.CreateDirectory(appStartMenuPath);
        }

        CreateShortcut(Environment.SpecialFolder.CommonStartMenu);
        Status.Content = "Starting GuardRail...";
        var finalProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = Path.Combine(_applicationRootFolder, "Latest", "GuardRail.exe")
            }
        };
        finalProcess.Start();
    }

    private void CreateShortcut(
        Environment.SpecialFolder specialFolder)
    {
        var folderPath = Environment.GetFolderPath(specialFolder);
        var shortcutPath = Path.Combine(folderPath, "GuardRail.lnk");
        if (folderPath.IsNullOrEmpty()
            || File.Exists(shortcutPath))
        {
            return;
        }

        var shell = new WshShell();
        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = Path.Combine(_applicationRootFolder, "Latest", "GuardRail.exe");
        shortcut.WorkingDirectory = _applicationRootFolder;
        shortcut.Description = "GuardRail Access Control";
        shortcut.Save();
    }
}