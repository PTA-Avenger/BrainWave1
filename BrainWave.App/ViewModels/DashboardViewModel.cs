using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace BrainWave.App.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string welcomeMessage = "Welcome back!";

        [ObservableProperty]
        private string latestNote = "Loading...";

        [ObservableProperty]
        private string mostUrgentTask = "Loading...";

        [ObservableProperty]
        private int streakCount;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task LoadDashboardAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Task.Delay(1000); // mock delay
                LatestNote = "Your last saved note goes here.";
                MostUrgentTask = "Finish writing the proposal.";
                StreakCount = 5;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToTasksAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.TaskListPage));
        }

        [RelayCommand]
        private async Task GoToRemindersAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.ReminderPage));
        }

        [RelayCommand]
        private async Task GoToBadgesAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.BadgePage));
        }

        [RelayCommand]
        private async Task GoToCollabAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.CollaborationPage));
        }

        [RelayCommand]
        private async Task GoToProfileAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.ProfilePage));
        }
    }
}
