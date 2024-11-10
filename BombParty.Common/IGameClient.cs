namespace BombParty.Common
{
    public interface IGameClient
    {
        Task ChangeName(string name);
        Task SendChatMessage(string text);

        Task SetInput(string text);
        Task SubmitInput(string text);
    }
}
