
using BombParty.Common;

namespace BombParty.Server.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly List<Player> _activePlayers = new();

        public PlayerService()
        {
            
        }

        public IEnumerable<Player> ActivePlayers => _activePlayers;

        public void AddPlayer(Player player)
        {
            if (_activePlayers.Any(p => p.Id == player.Id))
                throw new ArgumentException("Player with given ID already exists.");

            _activePlayers.Add(player);
        }

        public Player? GetPlayer(string playerId)
        {
            return _activePlayers.SingleOrDefault(p => p.Id == playerId);
        }

        public bool PlayerExists(string playerId)
        {
            return _activePlayers.Any(p => p.Id == playerId);
        }

        public void RemovePlayer(string playerId)
        {
            _activePlayers.RemoveAll(p => p.Id == playerId);
        }
    }
}
