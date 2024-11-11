using BombParty.Server.Models;

namespace BombParty.Server.Services
{
    public interface IRoomService
    {
        IEnumerable<Room> ActiveRooms { get; }

        void CreateRoom(Room room);
        Room? GetRoom(string roomId);
        void DestroyRoom(string roomId);

        bool IsPlayerInRoom(string playerId);
        Room? CurrentRoom(string playerId);
    }
}
