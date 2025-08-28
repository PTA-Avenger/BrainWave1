using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using BrainWave.App.Services;
using BrainWave.App.Models;
using Microsoft.Maui.Controls;
using System.IdentityModel.Tokens.Jwt;

namespace BrainWave.App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _api;
        private readonly AuthService _auth;

        public LoginViewModel(ApiService api, AuthService auth)
        {
            _api = api;
            _auth = auth;
        }
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var token = await _api.LoginAsync(new LoginRequest(Email, Password));
                if (string.IsNullOrWhiteSpace(token))
                {
                    ErrorMessage = "Invalid credentials.";
                    return;
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var sub = jwt.Subject;
                int userId = 0; int.TryParse(sub, out userId);
                await _auth.SaveAuthAsync(new AuthResponse { Token = token, UserID = userId, Email = Email });

                // Fetch user to check role
                var me = await _api.GetUserAsync(userId);
                var isAdmin = string.Equals(me?.Role, "Admin", System.StringComparison.OrdinalIgnoreCase);
                if (isAdmin)
                    await Shell.Current.GoToAsync("//Admin");
                else
                    await Shell.Current.GoToAsync("//Dashboard");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
