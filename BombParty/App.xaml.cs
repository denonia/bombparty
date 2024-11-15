﻿using BombParty.Extensions;
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

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Current.MainWindow is null)
                return;

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
