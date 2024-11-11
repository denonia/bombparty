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

            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<IRoomService, RoomService>();
            builder.Services.AddSingleton<IPlayerService, PlayerService>();
            builder.Services.AddSingleton<IDictionaryService, DictionaryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<GameHub>("game-hub");

            app.Run();
        }
    }
}
