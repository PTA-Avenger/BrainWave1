using BrainWave.App.Models;
using BrainWave.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    // The [ObservableObject] attribute is required for the toolkit to work its magic.
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public RegisterViewModel()
        {

        }
        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Properties using the ObservableProperty attribute will automatically generate
        // the property with change notification.
        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _role = "User";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string _errorMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSuccess))]
        private string _successMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);
        public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);
        public bool IsNotBusy => !IsBusy;

        // The [RelayCommand] attribute automatically creates the ICommand and the
        // asynchronous command logic, replacing the manual setup.
        [RelayCommand]
        private async Task RegisterAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and Password are required.";
                IsBusy = false;
                return;
            }
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                IsBusy = false;
                return;
            }

            try
            {
                var req = new RegisterRequest(_firstName, LastName, Email, Password, Role);
                var result = await _apiService.RegisterAsync(req);

                if (result == null)
                {
                    _errorMessage = "Registration failed. Please try again.";
                }
                else
                {
                    _successMessage = "Registration successful!";
                }
            }
            catch (System.Exception ex)
            {
                _errorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
