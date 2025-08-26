using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BrainWave.App.Models;
using BrainWave.App.Services;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    public partial class TaskDetailViewModel : ObservableObject
    {
        private readonly ApiService _api;
        private readonly AuthService _auth;

        [ObservableProperty]
        private TaskDto task;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public TaskDetailViewModel(ApiService api, AuthService auth)
        {
            _api = api;
            _auth = auth;
        }

        public async Task LoadTaskAsync(int taskId)
        {
            isBusy = true;
            errorMessage = string.Empty;
            try
            {
                var tasks = await _api.GetTasksAsync(_auth.CurrentUserId ?? 0);
                task = tasks?.FirstOrDefault(t => t.TaskID == taskId);
                OnPropertyChanged(nameof(Task));
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