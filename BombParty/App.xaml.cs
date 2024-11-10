using BombParty.Services;
using BombParty.ViewModels;
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
                .AddSingleton<GameService>()
                .AddTransient<GameViewModel>()
                .BuildServiceProvider();

            var gameService = services.GetRequiredService<GameService>();
            gameService.ConnectAsync().ConfigureAwait(false);

            var mainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<GameViewModel>()
            };

            mainWindow.Show();
        }
    }

}
