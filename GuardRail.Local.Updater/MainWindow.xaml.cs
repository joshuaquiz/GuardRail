using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using File = System.IO.File;

namespace GuardRail.Local.Updater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly bool _isFirstInstall;
    private readonly string _currentDir;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private InstallConfiguration _installConfiguration;

    /// <summary>
    /// Setup window.
    /// </summary>
    public MainWindow(CommandLineArguments commandLineArguments)
    {
        _isFirstInstall = !commandLineArguments.Any()
                          || commandLineArguments.ContainsKey(CommandLineArgumentType.FreshInstall);
        InitializeComponent();
        _currentDir = _isFirstInstall
            ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            : Environment.CurrentDirectory;
        _cancellationTokenSource = new CancellationTokenSource();
        Status.Content = "Checking for updates...";
        Loaded += OnLoaded;
        Unloaded += (_, _) => _cancellationTokenSource.Cancel();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (await HasUpdate())
        {
            if (!await UpdateDownloaded())
            {
                await DownloadUpdate();
            }

            await StartUpdate();
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

        var configuration = Encoding.Unicode.GetString(await GetData(new Uri(uriString, UriKind.Absolute)));
        _installConfiguration = configuration.FromJson<InstallConfiguration?>() ?? new InstallConfiguration(string.Empty, Array.Empty<InstallFile>(), string.Empty, string.Empty);
        return Assembly.GetExecutingAssembly().GetName().Version < new Version(_installConfiguration.LatestVersion);
    }

    private Task<bool> UpdateDownloaded() =>
        Task.FromResult(
            Directory.Exists(_currentDir + _installConfiguration.UpdateDirectory)
            && Directory.GetFiles(_currentDir + _installConfiguration.UpdateDirectory).Any());

    private async Task DownloadUpdate()
    {
        Status.Content = "Downloading...";
        ProgressBar.Maximum = _installConfiguration.InstallFiles.Count;
        await Task.WhenAll(_installConfiguration.InstallFiles.Select(SaveFile));
        Status.Content = "Downloading complete";
    }

    private static async Task<byte[]> GetData(Uri url)
    {
        using var client = new WebClient();
        return await client.DownloadDataTaskAsync(url);
    }

    private async Task SaveFile(InstallFile installFile)
    {
        await File.WriteAllBytesAsync(
            _currentDir + _installConfiguration.UpdateDirectory + installFile.LocalPath,
            await GetData(installFile.DownloadUri),
            _cancellationTokenSource.Token);
        ProgressBar.Value++;
    }

    private async Task StartUpdate()
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

        Status.Content = "Deleting old files...";
        var oldFiles = Directory.GetFiles(_currentDir);
        ProgressBar.Maximum = oldFiles.Length;
        await Task.WhenAll(
            oldFiles
                .Select(x =>
                    Task.Run(
                        () =>
                        {
                            if (!IsRestricted(x))
                            {
                                File.Delete(x);
                                ProgressBar.Value++;
                            }
                            else
                            {
                                ProgressBar.Maximum--;
                            }
                        })));
        Status.Content = "Installing new files...";
        ProgressBar.Value = 0;
        var newFiles = Directory.GetFiles(_currentDir + _installConfiguration.UpdateDirectory);
        ProgressBar.Maximum = newFiles.Length;
        await Task.WhenAll(
            newFiles
                .Select(x =>
                    Task.Run(
                        () =>
                        {
                            File.Move(x, x.Replace(_currentDir + _installConfiguration.UpdateDirectory, _currentDir));
                            ProgressBar.Value++;
                        })));
        if (_isFirstInstall)
        {
            CreateShortcut(Environment.SpecialFolder.Desktop);
            CreateShortcut(Environment.SpecialFolder.CommonStartup);
            var commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            var appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "GuardRail");
            if (!Directory.Exists(appStartMenuPath))
            {
                Directory.CreateDirectory(appStartMenuPath);
            }

            CreateShortcut(Environment.SpecialFolder.CommonStartMenu);
        }

        var finalProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = _installConfiguration.RestartCommand
            }
        };
        finalProcess.Start();
    }

    private void CreateShortcut(Environment.SpecialFolder folder)
    {
        var folderPath = Environment.GetFolderPath(folder);
        var link = (IShellLink)new ShellLink();
        link.SetDescription("GuardRail Access Control");
        link.SetWorkingDirectory(_currentDir + "\\GuardRail.exe");
        link.SetPath(_currentDir);
        var file = (IPersistFile)link;
        file.Save(Path.Combine(folderPath, "MyLink.lnk"), false);
    }

    private bool IsRestricted(string file) =>
        file.Equals(AppDomain.CurrentDomain.FriendlyName)
        || file.Contains(_currentDir + _installConfiguration.UpdateDirectory);
}

[ComImport]
[Guid("00021401-0000-0000-C000-000000000046")]
internal class ShellLink
{
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F9-0000-0000-C000-000000000046")]
internal interface IShellLink
{
    void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
    void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
    void Resolve(IntPtr hwnd, int fFlags);
    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}