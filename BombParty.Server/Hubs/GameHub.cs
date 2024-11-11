using BombParty.Common;
using BombParty.Common.Dtos;
using BombParty.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BombParty.Server.Hubs
{
    public class GameHub : Hub<IGameServer>, IGameClient, IDisposable
    {
        private readonly IPlayerService _playerService;
        private readonly IRoomService _roomService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IPlayerService playerService, IRoomService roomService,
            ILogger<GameHub> logger)
        {
            _playerService = playerService;
            _roomService = roomService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("{} has connected", UserId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _playerService.RemovePlayer(UserId);

            if (_roomService.IsPlayerInRoom(UserId))
                await LeaveRoom();

            _logger.LogInformation("{} has disconnected", UserId);

        }

        public async Task UpdateSettings(PlayerSettings playerSettings)
        {
            if (_playerService.PlayerExists(UserId))
            {
                _logger.LogInformation("{} has updated their settings", UserId);

                CurrentPlayer.Settings = playerSettings;
            }
            else
            {
                _logger.LogInformation("{} has shared their settings", UserId);

                var newPlayer = new Player(UserId);
                newPlayer.Settings = playerSettings;

                _playerService.AddPlayer(newPlayer);
                await SendLobbyUpdate(Clients.Caller);
            }
        }

        public async Task RequestActiveRooms()
        {
            _logger.LogInformation("{} has requested active rooms", UserId);

            await SendLobbyUpdate(Clients.Caller);
        }

        public async Task CreateRoom(CreateRoomDto createRoomDto)
        {
            var room = _roomService.CreateRoom(createRoomDto, CurrentPlayer);

            await Groups.AddToGroupAsync(UserId, room.GroupName);
            await Clients.Caller.JoinRoomResult(true, room.Settings);
            await Clients.Caller.UserPresence(CurrentPlayer);

            await SendLobbyUpdate(Clients.All);

#if DEBUG
            await Task.Delay(500);
            room.StartGame();
#endif
        }

        public async Task JoinRoom(string roomId, string? password)
        {
            var joinSuccess = _roomService.PlayerJoinRoom(CurrentPlayer, roomId, password);

            if (!joinSuccess)
            {
                await Clients.Caller.JoinRoomResult(joinSuccess, null);
                return;
            }

            var room = _roomService.CurrentRoom(UserId)!;

            await Clients.Caller.JoinRoomResult(joinSuccess, room.Settings);
            await Groups.AddToGroupAsync(UserId, room.GroupName);
            await Clients.Others.UserJoined(CurrentPlayer);
            foreach (var existingPlayer in room.Players)
                await Clients.Caller.UserPresence(existingPlayer);

            await SendLobbyUpdate(Clients.All);
        }

        public async Task LeaveRoom()
        {
            await OthersInRoom.UserLeft(UserId);

            var room = _roomService.CurrentRoom(UserId);

            _roomService.PlayerLeaveRoom(UserId);

            await Groups.RemoveFromGroupAsync(UserId, room.GroupName);

            await SendLobbyUpdate(Clients.All);
        }

        public async Task SendChatMessage(string text)
        {
            await CurrentRoom.ChatMessage(UserId, text);

            _logger.LogInformation("{}: {}", UserId, text);

            if (text == "/start")
            {
                var room = _roomService.CurrentRoom(UserId);
                room.StartGame();
            }
        }

        public async Task SetInput(string text)
        {
            await OthersInRoom.UserTyping(UserId, text);
        }

        public async Task SubmitInput(string text)
        {
            var right = _roomService.PlayerSubmitAnswer(UserId, text);
            await CurrentRoom.UserSubmittedAnswer(UserId, text, right);
        }

        private async Task SendLobbyUpdate(IGameServer target)
        {
            // TODO: create a group for lobby so we don't have to send this to everyone
            await target.ActiveRooms(_roomService.ActiveRooms
                .Select(r => new RoomDetailsDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    OwnerName = r.Owner.DisplayName,
                    RequiresPassword = r.RequiresPassword,
                    PlayerNames = r.Players.Select(p => p.DisplayName).ToArray(),
                    Settings = r.Settings
                }).ToList());
        }

        private string UserId => Context.ConnectionId;

        private Player CurrentPlayer => _playerService.GetPlayer(UserId) ?? throw new Exception("User not authenticated.");

        private IGameServer CurrentRoom
        {
            get
            {
                var room = _roomService.CurrentRoom(UserId) ?? throw new Exception("User is not in a room.");

                return Clients.Group(room.GroupName);
            }
        }

        private IGameServer OthersInRoom
        {
            get
            {
                var room = _roomService.CurrentRoom(UserId) ?? throw new Exception("User is not in a room.");

                return Clients.OthersInGroup(room.GroupName);
            }
        }
    }
}
