namespace BombParty.Common
{
    public interface IGameServer
    {
        Task UserPresence(string id, string? userName, int healthPoints);
        Task UserJoined(string id);
        Task UserChangedName(string id, string userName);
        Task UserLeft(string id);

        Task RoundStart(string currentUserId, string currentCombination);
        Task GameOver();
        Task UserHealthChanged(string id, int newHealth);

        Task UserTyping(string id, string text);
        Task UserSubmittedAnswer(string id, string text, bool right);
        Task ChatMessage(string senderId, string text);
    }
}
