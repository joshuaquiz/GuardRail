using GuardRail.Mobile.Client.Interfaces;
using GuardRail.Mobile.Client.Views;

namespace GuardRail.Mobile.Client;

public partial class App
{
    public App(
        IGuardRailStorage guardRailStorage,
        Login login)
    {
        if (guardRailStorage.GetUser() is null)
        {
            Windows[0].Page = login;
        }
        else
        {
            Windows[0].Page = new AppShell();
        }

        InitializeComponent();
        UserAppTheme = guardRailStorage.GetCurrentAppTheme(this).GetAwaiter().GetResult();
    }
}