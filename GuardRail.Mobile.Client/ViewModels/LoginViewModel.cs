using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.ApiModels.Responses;
using GuardRail.Mobile.Client.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Mobile.Client.ViewModels;

public partial class LoginViewModel(
        IServiceProvider serviceProvider)
    : BaseViewModel
{
    private readonly IGuardRailStorage _guardRailStorage = serviceProvider.GetRequiredService<IGuardRailStorage>();
    private readonly IGuardRailHttpClient _httpClient = serviceProvider.GetRequiredService<IGuardRailHttpClient>();

    public async Task LogIn(
        string username,
        string password)
    {
        IsBusy = true;
        try
        {
            var user = await _httpClient.PostData<MobileLoginResponse, object>(
                "/api/Account/LogIn",
                new
                {
                    Username = username,
                    Password = password
                },
                new CancellationTokenSource(
                        TimeSpan.FromMinutes(
                            1))
                    .Token);
            await _guardRailStorage.SetUser(user);
        }
        finally
        {
            IsBusy = false;
        }
    }
}