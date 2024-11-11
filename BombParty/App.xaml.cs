using BombParty.Common;
using BombParty.Extensions;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Game;
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
                .AddSingleton<SettingsStore>()
                .AddSingleton<NavigationStore>()
                .AddSingleton<GameService>()
                .AddViewModel<MainViewModel>()
                .AddViewModel<LobbyViewModel>()
                .AddViewModel<SettingsViewModel>()
                .AddViewModel<CreateRoomViewModel>()
                .AddViewModel<GameViewModel>()
                .BuildServiceProvider();

            services.GetRequiredService<NavigationService<LobbyViewModel>>().Navigate();

            var gameService = services.GetRequiredService<GameService>();
            gameService.ConnectAsync().ConfigureAwait(false);

            var settings = new PlayerSettings
            {
                UserName = "user1"
            };
            gameService.UpdateSettings(settings).ConfigureAwait(false);

            var mainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }
    }

}
