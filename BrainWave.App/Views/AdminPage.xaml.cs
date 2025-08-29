using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class AdminPage : ContentPage
    {
        private AdminViewModel _viewModel;

        public AdminPage()
        {
            InitializeComponent();
            _viewModel = new AdminViewModel();
            BindingContext = _viewModel;
        }

        private void OnUserRoleFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedRole = picker.SelectedItem?.ToString();
                _viewModel.ApplyUserRoleFilter(selectedRole);
            }
        }

        private void OnUserStatusFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedStatus = picker.SelectedItem?.ToString();
                _viewModel.ApplyUserStatusFilter(selectedStatus);
            }
        }

        private void OnAdminTaskStatusFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedStatus = picker.SelectedItem?.ToString();
                _viewModel.ApplyTaskStatusFilter(selectedStatus);
            }
        }

        private void OnAdminTaskPriorityFilterChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                var selectedPriority = picker.SelectedItem?.ToString();
                _viewModel.ApplyTaskPriorityFilter(selectedPriority);
            }
        }
    }
}