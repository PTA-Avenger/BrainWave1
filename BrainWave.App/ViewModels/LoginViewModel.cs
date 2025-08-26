using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
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
                // TODO: Call API /Auth/login endpoint
                await Task.Delay(1000); // mock delay

                if (Email == "test@example.com" && Password == "password")
                {
                    // Navigate to dashboard
                }
                else
                {
                    ErrorMessage = "Invalid credentials.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
