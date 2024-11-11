using BombParty.Extensions;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Lobby;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BombParty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
    }

}
