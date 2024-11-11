using BombParty.Common;

namespace BombParty.Server.Services
{
    public interface IPlayerService
    {
        IEnumerable<Player> ActivePlayers { get; }

        void AddPlayer(Player player);
        Player? GetPlayer(string playerId);
        bool PlayerExists(string playerId);
        void RemovePlayer(string playerId);
    }
}
