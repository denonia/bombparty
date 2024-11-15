using BombParty.Common;
using BombParty.Common.Dtos;

namespace BombParty.Services
{
    public interface IGameService
    {
        event Action<IList<RoomDetailsDto>>? OnActiveRooms;
        event Action<bool>? OnJoinRoomResult;
        
        event Action<Player>? OnUserPresence;
        event Action<Player>? OnUserJoined;
        event Action<string>? OnUserLeft;

        event Action<string, string>? OnRoundStart;
        event Action? OnGameOver;
        event Action<string, int>? OnHealthChanged;

        event Action<string, string>? OnUserTyping;
        event Action<string, string, bool>? OnAnswerSubmitted;
        event Action<string, string>? OnChatMessage;

        string ConnectionId { get; }

        RoomSettings RoomSettings { get; }

        Task ConnectAsync();

        Task UpdateSettings(PlayerSettings playerSettings);
        Task RequestActiveRooms();
        Task CreateRoom(CreateRoomDto createRoomDto);
        Task JoinRoom(string roomId, string? password);
        Task LeaveRoom();
        Task SendChatMessage(string text);
        Task SetInput(string text);
        Task SubmitInput(string text);
        Task ChangeName(string name);
    }
}
