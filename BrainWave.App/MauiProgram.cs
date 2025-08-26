using BrainWave.App.Services;
using BrainWave.App.Models;
using BrainWave.App.Views;
using BrainWave.App.ViewModels;


namespace BrainWave.App;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });


        // Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<LocalDatabase>();
        builder.Services.AddSingleton<SyncService>();


        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TaskListViewModel>();
        builder.Services.AddTransient<TaskDetailViewModel>();
        builder.Services.AddTransient<ReminderViewModel>();
        builder.Services.AddTransient<BadgeViewModel>();
        builder.Services.AddTransient<BrainWave.App.ViewModels.CollaborationViewModel>();


        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TaskListPage>();
        builder.Services.AddTransient<TaskDetailPage>();
        builder.Services.AddTransient<ReminderPage>();
        builder.Services.AddTransient<BadgePage>();
        builder.Services.AddTransient<CollaborationPage>();


        return builder.Build();
    }
}