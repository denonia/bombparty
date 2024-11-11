using BombParty.Server.Models;

namespace BombParty.Server.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _activeRooms = new();
        private readonly IEventService _eventService;

        public RoomService(IEventService eventService)
        {
            _eventService = eventService;
        }

        public IEnumerable<Room> ActiveRooms => _activeRooms;

        public void CreateRoom(Room room)
        {
            if (_activeRooms.Any(r => r.Id == room.Id))
                throw new ArgumentException("Room with given ID already exists.");

            _activeRooms.Add(room);

            _eventService.RegisterRoom(room);
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
