using BombParty.Common;
using BombParty.Common.Dtos;
using BombParty.Server.Models;

namespace BombParty.Server.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _activeRooms = new();
        private readonly IDictionaryService _dictionaryService;
        private readonly IEventService _eventService;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IDictionaryService dictionaryService, IEventService eventService, ILogger<RoomService> logger)
        {
            _dictionaryService = dictionaryService;
            _eventService = eventService;
            _logger = logger;
        }

        public IEnumerable<Room> ActiveRooms => _activeRooms;

        public Room CreateRoom(CreateRoomDto createRoomDto, Player owner)
        {
            var room = new Room(createRoomDto, owner, _dictionaryService.GetDictionary(createRoomDto.Settings.Language));

            if (_activeRooms.Any(r => r.Id == room.Id))
                throw new ArgumentException("Room with given ID already exists.");

            _activeRooms.Add(room);

            _eventService.RegisterRoom(room);

            _logger.LogInformation("{} has created room {}", owner.DisplayName, createRoomDto.Name);

            return room;
        }

        public Room? GetRoom(string roomId)
        {
            return _activeRooms.SingleOrDefault(r => r.Id.ToString() == roomId);
        }

        public void DestroyRoom(string roomId)
        {
            _eventService.UnregisterRoom(roomId);
            _activeRooms.RemoveAll(r => r.Id.ToString() == roomId);
        }

        public bool PlayerJoinRoom(Player player, string roomId, string? password)
        {
            var room = GetRoom(roomId);
            if (room is null)
                throw new ArgumentException("Room doesn't exist.");

            var result = room.AddPlayer(player, password);

            if (result)
                _logger.LogInformation("{} has joined room {}", player.DisplayName, room.Name);
            else
                _logger.LogInformation("{} has unsuccessfully tried to join room {}", player.DisplayName, room.Name);

            return result;
        }

        public bool PlayerSubmitAnswer(string playerId, string answer)
        {
            var room = CurrentRoom(playerId);
            var right = room.Game.SubmitAnswer(playerId, answer);

            _logger.LogInformation("{} has submitted answer: {} ({})", playerId, answer, right ? "+" : "-");

            return right;
        }

        public void PlayerLeaveRoom(string playerId)
        {
            var room = CurrentRoom(playerId)!;
            room.RemovePlayer(playerId);

            _logger.LogInformation("{} has left room {}", playerId, room.Name);

            if (room.Players.Count == 0)
                DestroyRoom(room.Id.ToString());
        }

        public bool IsPlayerInRoom(string playerId)
        {
            return _activeRooms.Any(r => r.IsPlayerInRoom(playerId));
        }

        public Room? CurrentRoom(string playerId)
        {
            return _activeRooms.SingleOrDefault(r => r.IsPlayerInRoom(playerId));
        }
    }
}
