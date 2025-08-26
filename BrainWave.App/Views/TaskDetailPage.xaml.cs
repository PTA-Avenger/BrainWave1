using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage()
        {
            InitializeComponent();
            BindingContext = App.Current.Handler.MauiContext.Services.GetService<TaskDetailViewModel>();
        }
    }
}
