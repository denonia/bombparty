using BombParty.Common.Dtos;

namespace BombParty.Common
{
    public interface IGameServer
    {
        Task ActiveRooms(IList<RoomDetailsDto> roomDtos);
        Task JoinRoomResult(bool success);

        Task UserPresence(Player player);
        Task UserJoined(Player player);
        Task UserLeft(string id);

        Task RoundStart(string currentUserId, string currentCombination);
        Task GameOver();
        Task UserHealthChanged(string id, int newHealth);

        Task UserTyping(string id, string text);
        Task UserSubmittedAnswer(string id, string text, bool right);
        Task ChatMessage(string senderId, string text);
    }
}
