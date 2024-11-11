using BombParty.Extensions;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Lobby;
using BombParty.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace BombParty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection()
                .AddSingleton(SynchronizationContext.Current!)
                .AddStores()
                .AddServices()
                .AddViewModels()
                .BuildServiceProvider();

            services.GetRequiredService<NavigationService<LobbyViewModel>>().Navigate();

            var gameService = services.GetRequiredService<GameService>();
            gameService.ConnectAsync().ConfigureAwait(false);

            var settingsStore = services.GetRequiredService<SettingsStore>();
            gameService.UpdateSettings(settingsStore.Settings.PlayerSettings).ConfigureAwait(false);

            var mainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var errorWindow = new ErrorWindow()
            {
                DataContext = new ErrorViewModel(e.Exception.ToString()),
                Owner = Current.MainWindow
            };

            errorWindow.ShowDialog();
        }
    }

}
