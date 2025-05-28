using GuardRail.Mobile.Client.Interfaces;
using GuardRail.Mobile.Client.Views;

namespace GuardRail.Mobile.Client;

public partial class App
{
    public App(
        IGuardRailStorage cookbookStorage,
        Login login)
    {
        if (cookbookStorage.GetUser() is null)
        {
            MainPage = login;
        }
        else
        {
            MainPage = new AppShell();
        }

        InitializeComponent();
        UserAppTheme = cookbookStorage.GetCurrentAppTheme(this).GetAwaiter().GetResult();
    }
}