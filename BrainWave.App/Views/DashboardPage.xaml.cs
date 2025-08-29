using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class DashboardPage : ContentPage
    {
        private DashboardViewModel _viewModel;

        public DashboardPage()
        {
            InitializeComponent();
            _viewModel = App.Current.Handler.MauiContext.Services.GetService<DashboardViewModel>();
            BindingContext = _viewModel;
        }

        private void OnStatusFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedStatus = picker.SelectedItem?.ToString();
                _viewModel.ApplyStatusFilter(selectedStatus);
            }
        }

        private void OnPriorityFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedPriority = picker.SelectedItem?.ToString();
                _viewModel.ApplyPriorityFilter(selectedPriority);
            }
        }
    }
}
