using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BrainWave.App.Models;
using BrainWave.App.Services;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    public partial class BadgeViewModel : ObservableObject
    {
        private readonly ApiService _api;
        private readonly AuthService _auth;

        [ObservableProperty]
        private ObservableCollection<BadgeDto> badges = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public BadgeViewModel(ApiService api, AuthService auth)
        {
            _api = api;
            _auth = auth;
        }

        [RelayCommand]
        public async Task LoadBadgesAsync()
        {
            if (isBusy) return;
            isBusy = true;
            errorMessage = string.Empty;
            try
            {
                var result = await _api.GetBadgesAsync();
                badges = new ObservableCollection<BadgeDto>(result ?? new());
                OnPropertyChanged(nameof(Badges));
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