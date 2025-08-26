using BrainWave.App.Views;
using BrainWave.App.Services;

namespace BrainWave.App
{
    public partial class App : Application
    {
        private readonly SyncService _sync;

        public App(SyncService sync)
        {
            _sync = sync;
            InitializeComponent();
            MainPage = new AppShell();
            _ = _sync.StartAsync();
        }
    }
}