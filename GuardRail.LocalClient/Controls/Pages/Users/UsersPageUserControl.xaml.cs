using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GuardRail.Core.Data.Models;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Controls.Pages.Users;

/// <summary>
/// Interaction logic for UsersUserControl.xaml
/// </summary>
public partial class UsersPageUserControl
{
    /// <summary>
    /// The viewModel.
    /// </summary>
    public UsersPageViewModel ViewModel;

    /// <summary>
    /// Interaction logic for UsersUserControl.xaml
    /// </summary>
    public UsersPageUserControl()
    {
        InitializeComponent();
        ViewModel = (UsersPageViewModel)DataContext;
        var view = (CollectionView)CollectionViewSource.GetDefaultView(UsersDisplay.ItemsSource);
        view.Filter = UserSearchFilter;
    }

    private bool UserSearchFilter(object obj)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
        {
            return true;
        }

        if (obj is User user)
        {
            return string.Join(" ", user.FirstName, user.LastName, user.Email, user.Phone).Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase);
        }

        throw new InvalidCastException("An item in the list was not the correct data type.");
    }

    private void SearchBox_OnTextChanged_TextChanged(object sender, TextChangedEventArgs e) =>
        CollectionViewSource.GetDefaultView(UsersDisplay.ItemsSource).Refresh();

    private void AddNewButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.EditingUser = new User
        {
            AccountGuid = App.Account.Guid
        };
        EditViewLabel.Content = "Add new user";
        EditGrid.Visibility = Visibility.Visible;
        UpdateLayout();
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (FirstNameTextBox.Text.IsNullOrWhiteSpace()
            || LastNameTextBox.Text.IsNullOrWhiteSpace()
            || PhoneTextBox.HasValidationError
            || EmailTextBox.HasValidationError)
        {
            MessageBox.Show(
                App.Host.Services.GetRequiredService<MainWindow>(),
                "The user must have a first and last name as well as an email and phone number.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK);
            return;
        }

        var dataStore = App.Host.Services.GetRequiredService<IDataStore>();
        if (ViewModel.EditingUser.Guid != default)
        {
            dataStore.UpdateExisting(ViewModel.EditingUser, App.CancellationTokenSource.Token);
        }
        else
        {
            dataStore.SaveNew(ViewModel.EditingUser, App.CancellationTokenSource.Token);
        }

        EditGrid.Visibility = Visibility.Collapsed;
        UpdateLayout();
    }

    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.EditingUser = (sender as Button)?.Tag as User;
        EditViewLabel.Content = "Edit user";

        EditGrid.Visibility = Visibility.Visible;
        UpdateLayout();
    }

    private void DeleteViewButton_OnClick(object sender, RoutedEventArgs e)
    {
        DeleteView.Visibility = Visibility.Collapsed;
        UpdateLayout();
    }

    private void YesDeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dataStore = App.Host.Services.GetRequiredService<IDataStore>();
        dataStore.DeleteExisting(ViewModel.EditingUser, App.CancellationTokenSource.Token);
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.EditingUser = (sender as Button)?.Tag as User;
        DeleteNameLabel.Content = ViewModel.EditingUser?.FirstName + " " + ViewModel.EditingUser?.LastName;
        DeleteView.Visibility = Visibility.Visible;
        UpdateLayout();
    }

    private void DoNothingPanel_OnClick(object sender, MouseButtonEventArgs e)
    {
    }
}