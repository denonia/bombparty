using BombParty.Services;
using BombParty.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BombParty.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddViewModel<T>(this IServiceCollection services) 
            where T : BaseViewModel
        {
            services.AddTransient<T>();
            services.AddSingleton<Func<T>>(s => s.GetRequiredService<T>);
            services.AddSingleton<NavigationService<T>>();

            return services;
        }
    }
}
