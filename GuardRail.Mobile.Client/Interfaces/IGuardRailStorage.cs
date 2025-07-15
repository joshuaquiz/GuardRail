using System.Threading.Tasks;
using GuardRail.ApiModels.Responses;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace GuardRail.Mobile.Client.Interfaces;

public interface IGuardRailStorage
{
    public Task<AppTheme> GetCurrentAppTheme(Application application);

    public Task SetAppTheme(AppTheme appTheme, Application application);

    public Task Empty();

    public Task SetUser(MobileLoginResponse user);

    public MobileLoginResponse? GetUser();
}