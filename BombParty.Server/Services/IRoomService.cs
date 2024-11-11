using BombParty.Common;
using BombParty.Common.Dtos;
using BombParty.Server.Models;

namespace BombParty.Server.Services
{
    public interface IRoomService
    {
        IEnumerable<Room> ActiveRooms { get; }

        Room CreateRoom(CreateRoomDto room, Player owner);
        Room? GetRoom(string roomId);
        void DestroyRoom(string roomId);

        bool PlayerJoinRoom(Player player, string roomId, string? password);
        bool PlayerSubmitAnswer(string playerId, string answer);
        void PlayerLeaveRoom(string playerId);

        bool IsPlayerInRoom(string playerId);
        Room? CurrentRoom(string playerId);
    }
}
