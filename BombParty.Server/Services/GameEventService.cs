using BombParty.Common;
using BombParty.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BombParty.Server.Services
{
    public class GameEventService : IDisposable
    {
        private readonly Game _game;
        private readonly IHubContext<GameHub, IGameServer> _hubContext;

        public GameEventService(Game game, IHubContext<GameHub, IGameServer> hubContext)
        {
            _game = game;
            _hubContext = hubContext;

            _game.OnRoundStart += OnRoundStart;
            _game.OnGameOver += OnGameOver;
            _game.OnHealthChanged += OnHealthChanged;
        }

        public void Dispose()
        {
            _game.OnRoundStart -= OnRoundStart;
            _game.OnGameOver -= OnGameOver;
            _game.OnHealthChanged -= OnHealthChanged;
        }

        private void OnRoundStart(string userId, string combination)
        {
            _hubContext.Clients.All.RoundStart(userId, combination);
        }

        private void OnGameOver()
        {
            _hubContext.Clients.All.GameOver();
        }

        private void OnHealthChanged(string userId, int newHealth)
        {
            _hubContext.Clients.All.UserHealthChanged(userId, newHealth);
        }
    }
}
