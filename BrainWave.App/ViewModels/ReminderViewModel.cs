using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BrainWave.App.Models;
using BrainWave.App.Services;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    public partial class ReminderViewModel : ObservableObject
    {
        private readonly ApiService _api;
        private readonly AuthService _auth;

        [ObservableProperty]
        private ObservableCollection<ReminderDto> reminders = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ReminderViewModel(ApiService api, AuthService auth)
        {
            _api = api;
            _auth = auth;
        }

        [RelayCommand]
        public async Task LoadRemindersAsync(int taskId)
        {
            if (isBusy) return;
            isBusy = true;
            errorMessage = string.Empty;
            try
            {
                var result = await _api.GetRemindersAsync(taskId);
                reminders = new ObservableCollection<ReminderDto>(result ?? new());
                OnPropertyChanged(nameof(Reminders));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}