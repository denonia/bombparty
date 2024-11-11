using BombParty.Common;
using System.Timers;

namespace BombParty.Server.Models
{
    public class Game : IDisposable
    {
        private readonly WordDictionary _dictionary;

        private List<Player> _players = new();

        int _currentPlayer = -1;
        string _currentCombination = string.Empty;
        System.Timers.Timer _timeoutTimer = new();

        public Game(Room room, List<Player> players, RoomSettings settings)
        {
            _players = players;
            Room = room;
            Settings = settings;

            _dictionary = new WordDictionary("english.txt");

            _timeoutTimer.Elapsed += new ElapsedEventHandler(OnRoundTimeout);
        }

        public event Action<string, string>? OnRoundStart;
        public event Action? OnGameOver;
        public event Action<string, int>? OnHealthChanged;

        public Room Room { get; }
        public RoomSettings Settings { get; }
        public bool Started { get; set; }
        public Player CurrentPlayer => _players[_currentPlayer];

        public void Start()
        {
            Started = true;

            _currentPlayer = -1;
            _timeoutTimer.Interval = Settings.RoundTime * 1000;
            _timeoutTimer.Enabled = true;

            foreach (var player in _players)
            {
                player.HealthPoints = Settings.StartHealthPoints;
                OnHealthChanged?.Invoke(player.Id, player.HealthPoints);
            }

            NextRound();
        }

        public void End()
        {
            Started = false;
            _timeoutTimer.Enabled = false;

            OnGameOver?.Invoke();
        }

        private void OnRoundTimeout(object? source, ElapsedEventArgs e)
        {
            var player = _players[_currentPlayer];

            player.Damage();
            OnHealthChanged?.Invoke(player.Id, player.HealthPoints);

            NextRound();
        }

        private void NextPlayer()
        {
            do
            {
                _currentPlayer = (_currentPlayer + 1) % _players.Count;
            }
            while (!CurrentPlayer.Alive);
        }

        public void NextRound()
        {
            _timeoutTimer.Stop();

            // End the game if:
            // 1. There are no players left.
            // 2. If there is only one player and they're dead.
            // 3. If there are 2+ players and everyone but one is dead.
            if (_players.Count == 0 
                || _players.Count == 1 && _players.All(p => !p.Alive)
                || _players.Count > 1 && _players.Count(p => p.Alive) == 1)
            {
                End();
                return;
            }

            _timeoutTimer.Start();

            NextPlayer();
            _currentCombination = _dictionary.GetRandomCombination();

            //_logger.LogInformation("New round {}", _currentCombination);

            OnRoundStart?.Invoke(_players[_currentPlayer].Id, _currentCombination);
        }

        public bool SubmitAnswer(string userId, string answer)
        {
            var player = _players[_currentPlayer];
            if (player.Id != userId)
                return false;

            var right = answer.Contains(_currentCombination) && _dictionary.Contains(answer);
            //if (right)
            //    _logger.LogInformation("{} has submitted the right answer: {}", player.DisplayName, answer);
            //else
            //    _logger.LogInformation("{} has submitted the wrong answer: {}", player.DisplayName, answer);

            return right;
        }

        public void Dispose()
        {
            _timeoutTimer.Dispose();
        }
    }
}
