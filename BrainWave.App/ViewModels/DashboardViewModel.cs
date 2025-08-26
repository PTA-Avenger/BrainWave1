using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

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
    }
}
