using BombParty.Common;
using BombParty.Server;
using Microsoft.AspNetCore.SignalR.Client;

namespace BombParty.ConsoleClient
{
    public class GameClient : IGameClient
    {
        private readonly HubConnection _hubConnection;

        private List<Player> _players = new();

        public GameClient(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            RegisterHandlers();
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                _players.Add(new Player(_hubConnection.ConnectionId!));

                await ChangeName("sigma");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Failed to connect to the server.");
            }
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<string, string, int>("UserPresence", (id, userName, healthPoints) =>
            {
                _players.Add(new Player(id)
                {
                    UserName = userName,
                    HealthPoints = healthPoints
                });
            });
            _hubConnection.On<string>("UserJoined", (id) =>
            {
                Console.WriteLine($"{id} has joined the game.");
                _players.Add(new Player(id));
            });
            _hubConnection.On<string, string>("UserChangedName", (id, newName) =>
            {
                Console.WriteLine($"{id} has set their name to {newName}.");
                GetPlayerById(id).UserName = newName;
            });
            _hubConnection.On<string>("UserLeft", (id) =>
            {
                Console.WriteLine($"{id} has left the game.");
                _players.RemoveAll(p => p.Id == id);
            });

            _hubConnection.On<string, string>("RoundStart", (id, combination) =>
            {
                Console.WriteLine($"New round! The combination is: {combination}. It's {GetPlayerById(id).DisplayName}'s turn.");
            });
            _hubConnection.On("GameOver", () =>
            {
                Console.WriteLine($"Game over!");
            });
            _hubConnection.On<string, int>("UserHealthChanged", (id, newHealth) =>
            {
                var player = GetPlayerById(id);
                player.HealthPoints = newHealth;
                
                if (newHealth == 0)
                    Console.WriteLine($"{player.DisplayName} is dead. Gg");
                else 
                    Console.WriteLine($"{player.DisplayName}'s health is {newHealth}");
            });

            _hubConnection.On<string, string>("UserTyping", (id, text) =>
            {
                Console.WriteLine($"{id} is typing: {text}");
                GetPlayerById(id).Input = text;
            });
            _hubConnection.On<string, string, bool>("UserSubmittedAnswer", (id, text, right) =>
            {
                if (right)
                    Console.WriteLine($"{id} has submitted the right answer: {text}");
                else
                    Console.WriteLine($"{id} has submitted the wrong answer: {text}");
            });

            _hubConnection.On<string, string>("ChatMessage", (senderId, text) =>
            {
                Console.WriteLine($"[Chat] {senderId}: {text}");
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
