using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class CollaborationPage : ContentPage
    {
        public CollaborationPage()
        {
            InitializeComponent();
            BindingContext = App.Current.Handler.MauiContext.Services.GetService<CollaborationViewModel>();
        }
    }
}
