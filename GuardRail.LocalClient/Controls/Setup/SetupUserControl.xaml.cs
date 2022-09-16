using System.Windows;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Controls.Setup;

/// <summary>
/// Interaction logic for SetupUserControl.xaml
/// </summary>
public partial class SetupUserControl
{
    private readonly IGuardRailApiClient _guardRailApiClient;

    /// <summary>
    /// Interaction logic for SetupUserControl.xaml
    /// </summary>
    public SetupUserControl()
    {
        _guardRailApiClient = App.Host.Services.GetRequiredService<IGuardRailApiClient>();
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (AccountNameTextBox.Text.IsNullOrWhiteSpace()
            || LocationTextBox.Text.IsNullOrWhiteSpace()
            || FirstNameTextBox.Text.IsNullOrWhiteSpace()
            || LastNameTextBox.Text.IsNullOrWhiteSpace()
            || PhoneTextBox.HasValidationError
            || EmailTextBox.HasValidationError)
        {
            MessageBox.Show(
                "Please fix all validation errors before preceding.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK);
        }
        else
        {
            await _guardRailApiClient.CreateNewAccount(
                AccountNameTextBox.Text,
                LocationTextBox.Text,
                FirstNameTextBox.Text,
                LastNameTextBox.Text,
                PhoneTextBox.Text,
                EmailTextBox.Text);
        }
    }
}