using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using BrainWave.App.Services;

namespace BrainWave.App.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _auth;

        [ObservableProperty]
        private string fullName = "";

        [ObservableProperty]
        private string email = "";

        [ObservableProperty]
        private string role = "";

        [ObservableProperty]
        private string profilePicture = "avatar_placeholder.png";

        public ProfileViewModel(AuthService auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

