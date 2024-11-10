using BombParty.Common;
using BombParty.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BombParty.Server.Hubs
{
    public class GameHub : Hub<IGameServer>, IGameClient, IDisposable
    {
        private readonly ILogger<GameHub> _logger;
        private readonly Game _game;

        public GameHub(Game game, ILogger<GameHub> logger, ILogger<Game> _gameLogger)
        {
            _game = game;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _game.AddPlayer(Context.ConnectionId);

            // TODO: properly sync until client is ready to receive messages
            await Task.Delay(100);

            foreach (var player in _game.Players)
            {
                await Clients.Caller.UserPresence(player.Id, player.UserName, player.HealthPoints);
            }

            await Clients.Others.UserJoined(Context.ConnectionId);

            _logger.LogInformation("{} has connected", Context.ConnectionId);

            await Task.Delay(100);

#if DEBUG
            if (!_game.Started)
                _game.Start();
#endif
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _game.RemovePlayer(Context.ConnectionId);

            await Clients.Others.UserLeft(Context.ConnectionId);

            _logger.LogInformation("{} has disconnected", Context.ConnectionId);

        }

        public async Task ChangeName(string name)
        {
            _game.ChangePlayerName(Context.ConnectionId, name);

            await Clients.All.UserChangedName(Context.ConnectionId, name);
        }

        public async Task SendChatMessage(string text)
        {
            await Clients.All.ChatMessage(Context.ConnectionId, text);

            _logger.LogInformation("{}: {}", Context.ConnectionId, text);

            if (text == "/start")
            {
                _game.Start();
            }
        }

        public async Task SetInput(string text)
        {
            await Clients.Others.UserTyping(Context.ConnectionId, text);
        }

        public async Task SubmitInput(string text)
        {
            var right = _game.SubmitAnswer(Context.ConnectionId, text);
            await Clients.All.UserSubmittedAnswer(Context.ConnectionId, text, right);

            if (right)
                _game.NextRound();
        }
    }
}
