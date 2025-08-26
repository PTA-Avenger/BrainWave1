using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage()
        {
            InitializeComponent();
            BindingContext = App.Current.Handler.MauiContext.Services.GetService<TaskListViewModel>();
            Loaded += async (s, e) =>
            {
                if (BindingContext is TaskListViewModel vm)
                    await vm.LoadTasksAsync();
            };
        }
    }
}
