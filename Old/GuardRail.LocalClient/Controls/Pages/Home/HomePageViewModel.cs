using System;
using System.Collections.ObjectModel;

namespace GuardRail.LocalClient.Controls.Pages.Home;

/// <summary>
/// View model for the home page user control.
/// </summary>
public sealed class HomePageViewModel
{
    /// <summary>
    /// For viewing logs in the application.
    /// </summary>
    public ObservableCollection<HomePageLogItem> Logs { get; set; } = new();

    /// <summary>
    /// Add a log to the display.
    /// </summary>
    /// <param name="timeStamp">When the log happened.</param>
    /// <param name="log">The log itself.</param>
    public void AddLog(DateTime timeStamp, string log) =>
        Logs.Add(
            new HomePageLogItem
            {
                TimeStamp = timeStamp.ToString("HH:mm:ss"),
                Log = log
            });
}