using System;
using GuardRail.Mobile.Client.Exceptions;
using GuardRail.Mobile.Client.ViewModels;
using Microsoft.Maui.Controls;

namespace GuardRail.Mobile.Client.Views;

public partial class Login
{
    private LoginViewModel ViewModel { get; set; }

    public Login(
        LoginViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = ViewModel;
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs args)
    {
        /*WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri("https://mysite.com/mobileauth/Microsoft"),
            new Uri("myapp://"));
        string accessToken = authResult?.AccessToken;*/
        try
        {
            await ViewModel.LogIn(
                Username.Text,
                Password.Text);
            Application.Current!.MainPage = new AppShell();
        }
        catch (NoInternetException e)
        {
            await DisplayAlert(
                "Connection issue",
                e.Message,
                "OK");
        }
        catch (Exception e)
        {
            await DisplayAlert(
                "Ops",
                e.Message,
                "OK");
        }
    }

    private void Google_OnClicked(object? sender, EventArgs e)
    {
        /*WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri("https://mysite.com/mobileauth/Microsoft"),
            new Uri("myapp://"));
        string accessToken = authResult?.AccessToken;*/
    }

    private void Facebook_OnClicked(object? sender, EventArgs e)
    {
        /*WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri("https://mysite.com/mobileauth/Microsoft"),
            new Uri("myapp://"));
        string accessToken = authResult?.AccessToken;*/
    }
}