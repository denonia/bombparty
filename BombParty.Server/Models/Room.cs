using BombParty.Common;
using BombParty.Common.Dtos;

namespace BombParty.Server.Models
{
    public class Room
    {
        private readonly List<Player> _players = new();

        public Room(CreateRoomDto dto, Player owner, WordDictionary dictionary)
        {
            OwnerId = owner.Id;
            Name = dto.Name;
            Password = dto.Password;
            Settings = dto.Settings;

            _players.Add(owner);

            Game = new Game(this, dictionary);
            Game.OnRoundStart += OnGameRoundStart;
            Game.OnGameOver += OnGameGameOver;
            Game.OnHealthChanged += OnGameHealthChanged;
        }

        public event Action<string, string, string>? OnRoundStart;
        public event Action<string>? OnGameOver;
        public event Action<string, string, int>? OnHealthChanged;

        public IList<Player> Players => _players;
        public Player Owner => _players.Single(p => p.Id == OwnerId);

        public Guid Id { get; set; } = Guid.NewGuid();
        public string GroupName => "room-" + Id;

        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        public bool RequiresPassword => !string.IsNullOrEmpty(Password);

        public RoomSettings Settings { get; set; }
        public Game Game { get; private set; }

        public void StartGame()
        {
            Game.Start();
        }

        public bool AddPlayer(Player player, string? password)
        {
            if (RequiresPassword && password != Password)
                return false;

            player.HealthPoints = Settings.StartHealthPoints;

            _players.Add(player);
            return true;
        }

        public void RemovePlayer(string playerId)
        {
            if (_players.RemoveAll(p => p.Id == playerId) > 0)
                Game.NextRound();

            // Transfer the ownership
            if (playerId == OwnerId && _players.Count != 0)
            {
                OwnerId = _players[0].Id;
            }
        }

        public bool IsPlayerInRoom(string playerId)
        {
            return _players.Any(p => p.Id == playerId);
        }

        private void OnGameRoundStart(string userId, string combination)
        {
            OnRoundStart?.Invoke(GroupName, userId, combination);
        }

        private void OnGameGameOver()
        {
            OnGameOver?.Invoke(GroupName);
        }

        private void OnGameHealthChanged(string userId, int newHealth)
        {
            OnHealthChanged?.Invoke(GroupName, userId, newHealth);
        }
    }
}
