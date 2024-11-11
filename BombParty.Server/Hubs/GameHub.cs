using BombParty.Common;
using BombParty.Common.Dtos;
using BombParty.Server.Models;
using BombParty.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BombParty.Server.Hubs
{
    public class GameHub : Hub<IGameServer>, IGameClient, IDisposable
    {
        private readonly IPlayerService _playerService;
        private readonly IRoomService _roomService;
        private readonly IDictionaryService _dictionaryService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(
            IPlayerService playerService,
            IRoomService roomService,
            IDictionaryService dictionaryService,
            ILogger<GameHub> logger, ILogger<Game> _gameLogger)
        {
            _playerService = playerService;
            _roomService = roomService;
            _dictionaryService = dictionaryService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("{} has connected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _playerService.RemovePlayer(Context.ConnectionId);

            if (_roomService.IsPlayerInRoom(Context.ConnectionId))
                await LeaveRoom();

            _logger.LogInformation("{} has disconnected", Context.ConnectionId);

        }

        public async Task UpdateSettings(PlayerSettings playerSettings)
        {
            if (_playerService.PlayerExists(Context.ConnectionId))
            {
                _logger.LogInformation("{} has updated their settings", Context.ConnectionId);

                var existingPlayer = _playerService.GetPlayer(Context.ConnectionId)!;
                existingPlayer.Settings = playerSettings;
            }
            else
            {
                _logger.LogInformation("{} has shared their settings", Context.ConnectionId);

                var newPlayer = new Player(Context.ConnectionId);
                newPlayer.Settings = playerSettings;

                _playerService.AddPlayer(newPlayer);
                await SendLobbyUpdate();
            }
        }


        public async Task RequestActiveRooms()
        {
            // await SendLobbyUpdate();

            _logger.LogInformation("{} has requested active rooms", Context.ConnectionId);

            await Clients.Caller.ActiveRooms(_roomService.ActiveRooms
                .Select(r => new RoomDetailsDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    OwnerName = r.Owner.DisplayName,
                    PlayerNames = r.Players.Select(p => p.DisplayName).ToArray(),
                    Settings = r.Settings
                }).ToList());
        }

        public async Task CreateRoom(CreateRoomDto createRoomDto)
        {
            _logger.LogInformation("{} has created room {}", Context.ConnectionId, createRoomDto.Name);

            var owner = _playerService.GetPlayer(Context.ConnectionId);
            var room = new Room(createRoomDto, owner, _dictionaryService.GetDictionary(createRoomDto.Settings.Language));
            _roomService.CreateRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, room.GroupName);

            await Clients.Caller.JoinRoomResult(true, room.Settings);
            await Clients.Caller.UserPresence(owner);

            await SendLobbyUpdate();

#if DEBUG
            await Task.Delay(500);
            room.StartGame();
#endif
        }

        public async Task JoinRoom(string roomId, string? password)
        {
            var room = _roomService.GetRoom(roomId);
            if (room is null)
                throw new Exception("Room doesn't exist.");

            var player = _playerService.GetPlayer(Context.ConnectionId);
            if (player is null)
                throw new Exception("User not authenticated.");

            var joinSuccess = room.AuthenticatePlayer(player, password);
            if (joinSuccess)
                await Groups.AddToGroupAsync(Context.ConnectionId, room.GroupName);

            await Clients.Caller.JoinRoomResult(joinSuccess, room.Settings);

            foreach (var existingPlayer in room.Players)
            {
                await Clients.Caller.UserPresence(existingPlayer);
            }
            await Clients.Others.UserJoined(player);

            await SendLobbyUpdate();
        }

        public async Task LeaveRoom()
        {
            await OthersInRoom.UserLeft(Context.ConnectionId);

            var room = _roomService.CurrentRoom(Context.ConnectionId)!;
            room.RemovePlayer(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.GroupName);

            if (room.Players.Count() == 0)
                _roomService.DestroyRoom(room.Id.ToString());

            _logger.LogInformation("{} has left room {}", Context.ConnectionId, room.Name);

            await SendLobbyUpdate();
        }

        public async Task SendChatMessage(string text)
        {
            await CurrentRoom.ChatMessage(Context.ConnectionId, text);

            _logger.LogInformation("{}: {}", Context.ConnectionId, text);

            if (text == "/start")
            {
                var room = _roomService.CurrentRoom(Context.ConnectionId);
                room.StartGame();
            }
        }

        public async Task SetInput(string text)
        {
            await OthersInRoom.UserTyping(Context.ConnectionId, text);
        }

        public async Task SubmitInput(string text)
        {
            var room = _roomService.CurrentRoom(Context.ConnectionId);
            var right = room.Game.SubmitAnswer(Context.ConnectionId, text);
            await CurrentRoom.UserSubmittedAnswer(Context.ConnectionId, text, right);

            if (right)
                room.Game.NextRound();
        }

        private async Task SendLobbyUpdate()
        {
            // TODO: create a group for lobby so we don't have to send this to everyone
            await Clients.All.ActiveRooms(_roomService.ActiveRooms
                .Select(r => new RoomDetailsDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    OwnerName = r.Owner.DisplayName,
                    PlayerNames = r.Players.Select(p => p.DisplayName).ToArray(),
                    Settings = r.Settings
                }).ToList());
        }

        private IGameServer CurrentRoom
        {
            get
            {
                var room = _roomService.CurrentRoom(Context.ConnectionId) ?? throw new Exception("User is not in a room.");

                return Clients.Group(room.GroupName);
            }
        }

        private IGameServer OthersInRoom
        {
            get
            {
                var room = _roomService.CurrentRoom(Context.ConnectionId) ?? throw new Exception("User is not in a room.");

                return Clients.OthersInGroup(room.GroupName);
            }
        }
    }
}
