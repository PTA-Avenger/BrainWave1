using BrainWave.App.Views;
using Microsoft.Maui.Controls;

namespace BrainWave.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register all pages that will be navigated to by name
            // even if they are already the initial content of the Shell.
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(TaskListPage), typeof(TaskListPage));
            Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
            Routing.RegisterRoute(nameof(ReminderPage), typeof(ReminderPage));
            Routing.RegisterRoute(nameof(BadgePage), typeof(BadgePage));
            Routing.RegisterRoute(nameof(CollaborationPage), typeof(CollaborationPage));
        }
    }
}
