using BombParty.Common.Dtos;

namespace BombParty.Common
{
    public interface IGameClient
    {
        Task UpdateSettings(PlayerSettings playerSettings);
        Task RequestActiveRooms();
        Task CreateRoom(CreateRoomDto createRoomDto);
        Task JoinRoom(string roomId, string? password);
        Task LeaveRoom();

        Task SendChatMessage(string text);

        Task SetInput(string text);
        Task SubmitInput(string text);
    }
}
