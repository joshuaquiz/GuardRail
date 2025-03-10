using GuardRail.Mobile.Client.ViewModels;

namespace GuardRail.Mobile.Client.Views;

public partial class HomePage
{
    public HomePage(
        HomePageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}