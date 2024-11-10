using BombParty.Server.Hubs;
using BombParty.Server.Services;

namespace BombParty.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<Game>();
            builder.Services.AddSingleton<GameEventService>();

            var app = builder.Build();

            app.Services.GetRequiredService<GameEventService>();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<GameHub>("game-hub");

            app.Run();
        }
    }
}
