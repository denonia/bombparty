using BombParty.Common;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BombParty.Server.Models
{
    public class Game : IDisposable
    {
        private readonly WordDictionary _dictionary;

        private int _currentPlayer = -1;
        private List<string> _usedWords = new();
        private string _currentCombination = string.Empty;
        private Timer _timeoutTimer = new();

        public Game(Room room, WordDictionary dictionary)
        {
            Room = room;

            _dictionary = dictionary;

            _timeoutTimer.Elapsed += new ElapsedEventHandler(OnRoundTimeout);
        }

        public event Action<string, string>? OnRoundStart;
        public event Action? OnGameOver;
        public event Action<string, int>? OnHealthChanged;

        public bool Started { get; set; }

        public Room Room { get; }
        public IList<Player> Players => Room.Players;
        public Player CurrentPlayer => Players[_currentPlayer];

        public void Start()
        {
            Started = true;

            _usedWords.Clear();
            _currentPlayer = -1;
            _timeoutTimer.Interval = Room.Settings.RoundTime * 1000;
            _timeoutTimer.Enabled = true;

            foreach (var player in Players)
            {
                player.HealthPoints = Room.Settings.StartHealthPoints;
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

        private void NextPlayer()
        {
            do
            {
                _currentPlayer = (_currentPlayer + 1) % Players.Count;
            }
            while (!CurrentPlayer.Alive);
        }

        public void NextRound()
        {
            if (!Started)
                return;

            _timeoutTimer.Stop();

            // End the game if:
            // 1. There are no players left.
            // 2. If there is only one player and they're dead.
            // 3. If there are 2+ players and everyone but one is dead.
            if (Players.Count == 0 
                || Players.Count == 1 && Players.All(p => !p.Alive)
                || Players.Count > 1 && Players.Count(p => p.Alive) == 1)
            {
                End();
                return;
            }

            _timeoutTimer.Start();

            NextPlayer();
            _currentCombination = _dictionary.GetRandomCombination();

            OnRoundStart?.Invoke(Players[_currentPlayer].Id, _currentCombination);
        }

        public bool SubmitAnswer(string userId, string answer)
        {
            var player = Players[_currentPlayer];
            if (player.Id != userId)
                return false;

            var right = answer.Contains(_currentCombination) && _dictionary.Contains(answer) && !_usedWords.Contains(answer);

            if (right)
            {
                _usedWords.Add(answer);
                NextRound();
            }

            return right;
        }

        private void OnRoundTimeout(object? source, ElapsedEventArgs e)
        {
            var player = Players[_currentPlayer];

            player.Damage();
            OnHealthChanged?.Invoke(player.Id, player.HealthPoints);

            NextRound();
        }

        public void Dispose()
        {
            _timeoutTimer.Dispose();
        }
    }
}
