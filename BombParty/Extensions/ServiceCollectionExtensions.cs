using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Game;
using BombParty.ViewModels.Lobby;
using Microsoft.Extensions.DependencyInjection;

namespace BombParty.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStores(this IServiceCollection services)
        {
            services
                .AddSingleton<ISettingsStore, SettingsStore>()
                .AddSingleton<INavigationStore, NavigationStore>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IGameService, GameService>()
                .AddSingleton<IThemeService, ThemeService>()
                .AddSingleton<IPasswordEntryService, PasswordEntryService>()
                .AddSingleton<IAudioService, AudioService>();

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services
                .AddViewModel<MainViewModel>()
                .AddViewModel<LobbyViewModel>()
                .AddViewModel<SettingsViewModel>()
                .AddViewModel<CreateRoomViewModel>()
                .AddViewModel<GameViewModel>();

            return services;
        }

        public static IServiceCollection AddViewModel<T>(this IServiceCollection services) 
            where T : BaseViewModel
        {
            services.AddTransient<T>();
            services.AddSingleton<Func<T>>(s => s.GetRequiredService<T>);
            services.AddSingleton<INavigationService<T>, NavigationService<T>>();

            return services;
        }
    }
}
