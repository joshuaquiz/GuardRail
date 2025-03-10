using GuardRail.Mobile.Client.Views;
using Microsoft.Maui.Controls;

namespace GuardRail.Mobile.Client;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Login), typeof(Login));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        /*Routing.RegisterRoute(nameof(ProfileHome), typeof(ProfileHome));
        Routing.RegisterRoute(nameof(SearchHome), typeof(SearchHome));
        Routing.RegisterRoute(nameof(MyCookbookHome), typeof(MyCookbookHome));*/
    }
}