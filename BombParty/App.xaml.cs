using BombParty.Extensions;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Lobby;
using BombParty.Views;
using BombParty.Windows;
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
#if RELEASE
            DispatcherUnhandledException += OnDispatcherUnhandledException;
#endif
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection()
                .AddSingleton(Application.Current)
                .AddSingleton(SynchronizationContext.Current!)
                .AddStores()
                .AddServices()
                .AddViewModels()
                .BuildServiceProvider();

            services.GetRequiredService<INavigationService<LobbyViewModel>>().Navigate();

            var gameService = services.GetRequiredService<IGameService>();
            var settingsStore = services.GetRequiredService<ISettingsStore>();

            Task.Run(async () =>
            {
                await gameService.ConnectAsync();
                await gameService.UpdateSettings(settingsStore.Settings.PlayerSettings);
            });

            var mainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }

#if RELEASE
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Current.MainWindow is null || !Current.MainWindow.IsVisible)
                return;

            e.Handled = true;

            var errorWindow = new ErrorWindow()
            {
                DataContext = new ErrorViewModel(e.Exception.ToString()),
                Owner = Current.MainWindow
            };

            errorWindow.ShowDialog();
        }
#endif
    }

}
