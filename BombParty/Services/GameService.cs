using BombParty.Common;
using BombParty.Common.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Windows;

namespace BombParty.Services
{
    public class GameService : IGameClient
    {
        private readonly HubConnection _hubConnection;

        private List<Player> _players = new();

        public GameService(SettingsStore settingsStore)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{settingsStore.Settings.ServerAddress}/game-hub")
                .Build();

            RegisterHandlers();
        }

        public event Action<IList<RoomDetailsDto>>? OnActiveRooms;
        public event Action<bool>? OnJoinRoomResult;
        
        public event Action<string, string?, int>? OnUserPresence;
        public event Action<Player>? OnUserJoined;
        public event Action<string>? OnUserLeft;

        public event Action<string, string>? OnRoundStart;
        public event Action? OnGameOver;
        public event Action<string, int>? OnHealthChanged;

        public event Action<string, string>? OnUserTyping;
        public event Action<string, string, bool>? OnAnswerSubmitted;
        public event Action<string, string>? OnChatMessage;

        public string ConnectionId => _hubConnection.ConnectionId;

        public RoomSettings RoomSettings { get; private set; } = new();

        public async Task ConnectAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Failed to connect to the server.", "Error");
                Environment.Exit(0);
            }
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<IList<RoomDetailsDto>>("ActiveRooms", (roomDtos) => OnActiveRooms?.Invoke(roomDtos));

            _hubConnection.On<bool, RoomSettings?>("JoinRoomResult", (success, roomSettings) =>
            {
                OnJoinRoomResult?.Invoke(success);

                if (success && roomSettings.HasValue)
                    RoomSettings = roomSettings.Value;
            });

            _hubConnection.On<Player>("UserPresence", (player) =>
            {
                _players.Add(player);
                OnUserPresence?.Invoke(player.Id, player.Settings.UserName, player.HealthPoints);
            });

            _hubConnection.On<Player>("UserJoined", (player) =>
            {
                _players.Add(player);
                OnUserJoined?.Invoke(player);
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

        public async Task UpdateSettings(PlayerSettings playerSettings)
        {
            await _hubConnection.SendAsync("UpdateSettings", playerSettings);
        }

        public async Task RequestActiveRooms()
        {
            await _hubConnection.SendAsync("RequestActiveRooms");
        }

        public async Task CreateRoom(CreateRoomDto createRoomDto)
        {
            await _hubConnection.SendAsync("CreateRoom", createRoomDto);
            _players.Clear();
        }

        public async Task JoinRoom(string roomId, string? password)
        {
            await _hubConnection.SendAsync("JoinRoom", roomId, password);
            _players.Clear();
        }

        public async Task LeaveRoom()
        {
            await _hubConnection.SendAsync("LeaveRoom");
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
