using System;
using Microsoft.Maui.Controls;

namespace BrainWave.App.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // The EmailEntry and PasswordEntry are now accessible directly
            // because they are automatically created by the InitializeComponent() call.
            await DisplayAlert("Login", $"Logging in as {EmailEntry.Text}", "OK");

            // Navigate to Dashboard after successful login
            await Shell.Current.GoToAsync(nameof(DashboardPage));
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Navigate to RegisterPage
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}
