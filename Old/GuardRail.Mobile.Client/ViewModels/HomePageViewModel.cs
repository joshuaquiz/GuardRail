using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using GuardRail.Mobile.Client.Interfaces;

namespace GuardRail.Mobile.Client.ViewModels;

public partial class HomePageViewModel : BaseViewModel
{
    private readonly INfcHandler _nfcHandler;

    public HomePageViewModel(
        INfcHandler nfcHandler)
    {
        Title = "GuardRail Verification";
        _nfcHandler = nfcHandler;
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task<bool> HandleTagTrigger(
        CancellationToken cancellationToken) =>
        await _nfcHandler.HandleTagTrigger(
            Guid.NewGuid(),
            cancellationToken);
}