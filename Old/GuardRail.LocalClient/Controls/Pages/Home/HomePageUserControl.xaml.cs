using System;
using System.Threading.Tasks;

namespace GuardRail.LocalClient.Controls.Pages.Home;

/// <summary>
/// Interaction logic for HomePageUserControl.xaml
/// </summary>
public partial class HomePageUserControl
{
    /// <summary>
    /// The viewModel.
    /// </summary>
    public HomePageViewModel ViewModel;

    /// <summary>
    /// Interaction logic for HomePageUserControl.xaml
    /// </summary>
    public HomePageUserControl()
    {
        InitializeComponent();
        ViewModel = (HomePageViewModel)DataContext;
    }

    /// <summary>
    /// Add a log to the display.
    /// </summary>
    /// <param name="timeStamp">When the log happened.</param>
    /// <param name="log">The log itself.</param>
    public async Task AddLogAsync(DateTime timeStamp, string log) =>
        await Dispatcher.InvokeAsync(() =>
            ViewModel.AddLog(
                timeStamp,
                log));
}