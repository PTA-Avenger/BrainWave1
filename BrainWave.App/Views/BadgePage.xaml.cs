using System;
using Microsoft.Maui.Controls;
using BrainWave.App.ViewModels;

namespace BrainWave.App.Views
{
    public partial class BadgePage : ContentPage
    {
        public BadgePage()
        {
            InitializeComponent();
            BindingContext = App.Current.Handler.MauiContext.Services.GetService<BadgeViewModel>();
        }
    }
}
