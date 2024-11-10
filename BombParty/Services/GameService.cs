using BombParty.Common;
using BombParty.Server;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace BombParty.Services
{
    public class GameService : IGameClient
    {
        private readonly HubConnection _hubConnection;

        private List<Player> _players = new();

        public GameService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7173/game-hub")
                .Build();

            RegisterHandlers();
        }

        public event Action<string, string>? OnRoundStart;
        public event Action? OnGameOver;
        public event Action<string, int>? OnHealthChanged;
        public event Action<string, string>? OnChatMessage;
        public event Action<string, string?, int>? OnUserPresence;
        public event Action<string>? OnUserJoined;
        public event Action<string>? OnUserLeft;
        public event Action<string, string>? OnUserNameChanged;
        public event Action<string, string>? OnUserTyping;
        public event Action<string, string, bool>? OnAnswerSubmitted;

        public string ConnectionId => _hubConnection.ConnectionId;

        public async Task ConnectAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Failed to connect to the server.");
            }
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<string, string?, int>("UserPresence", (id, userName, healthPoints) =>
            {
                _players.Add(new Player(id)
                {
                    UserName = userName,
                    HealthPoints = healthPoints
                });
                OnUserPresence?.Invoke(id, userName, healthPoints);
            });

            _hubConnection.On<string>("UserJoined", (id) =>
            {
                _players.Add(new Player(id));
                OnUserJoined?.Invoke(id);
            });

            _hubConnection.On<string, string>("UserChangedName", (id, newName) =>
            {
                GetPlayerById(id).UserName = newName;
                OnUserNameChanged?.Invoke(id, newName);
            });

            _hubConnection.On<string>("UserLeft", (id) =>
            {
                _players.RemoveAll(p => p.Id == id);
                OnUserLeft?.Invoke(id);
            });

            _hubConnection.On<string, string>("RoundStart", (id, combination) =>
            {
                OnRoundStart?.Invoke(id, combination);
            });

            _hubConnection.On("GameOver", () =>
            {
                OnGameOver?.Invoke();
            });

            _hubConnection.On<string, int>("UserHealthChanged", (id, newHealth) =>
            {
                var player = GetPlayerById(id);
                player.HealthPoints = newHealth;
                OnHealthChanged?.Invoke(id, newHealth);
            });

            _hubConnection.On<string, string>("UserTyping", (id, text) =>
            {
                GetPlayerById(id).Input = text;
                OnUserTyping?.Invoke(id, text);
            });

            _hubConnection.On<string, string, bool>("UserSubmittedAnswer", (id, text, right) =>
            {
                OnAnswerSubmitted?.Invoke(id, text, right);
            });

            _hubConnection.On<string, string>("ChatMessage", (senderId, text) =>
            {
                var player = GetPlayerById(senderId);

                OnChatMessage?.Invoke(player.DisplayName, text);
            });
        }

        public async Task SendChatMessage(string text)
        {
            await _hubConnection.SendAsync("SendChatMessage", text);
        }

        public async Task SetInput(string text)
        {
            await _hubConnection.SendAsync("SetInput", text);
        }

        public async Task SubmitInput(string text)
        {
            await _hubConnection.SendAsync("SubmitInput", text);
        }

        public async Task ChangeName(string name)
        {
            await _hubConnection.SendAsync("ChangeName", name);
        }

        private Player GetPlayerById(string id) => _players.Single(p => p.Id == id);
    }
}
