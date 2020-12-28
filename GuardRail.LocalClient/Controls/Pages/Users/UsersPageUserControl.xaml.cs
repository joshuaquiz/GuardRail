using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Interfaces;
using GuardRail.LocalClient.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Controls.Pages.Users
{
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
                return user.GetSearchString().Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase);
            }

            throw new InvalidCastException("An item in the list was not the correct data type.");
        }

        private void SearchBox_OnTextChanged_TextChanged(object sender, TextChangedEventArgs e) =>
            CollectionViewSource.GetDefaultView(UsersDisplay.ItemsSource).Refresh();

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FirstNameTextBox.Text.IsNullOrWhiteSpace()
                || LastNameTextBox.Text.IsNullOrWhiteSpace()
                || PhoneTextBox.HasValidationError
                || EmailTextBox.HasValidationError)
            {
                MessageBox.Show(
                    App.ServiceProvider.GetRequiredService<MainWindow>(),
                    "The user must have a first and last name as well as an email and phone number.",
                    "Validation error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }

            var user = new User
            {
                Account = App.Account,
                Email = EmailTextBox.Text,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Password = PasswordTextBox.Text,
                Phone = PhoneTextBox.Text,
                Username = UsernameTextBox.Text
            };
            var dataStore = App.ServiceProvider.GetRequiredService<IDataStore>();
            dataStore.SaveNew(user, App.CancellationTokenSource.Token);
        }
    }
}