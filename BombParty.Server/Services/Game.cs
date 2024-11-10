using System.Timers;

namespace BombParty.Server.Services
{
    public class Game : IDisposable
    {
        private readonly WordDictionary _dictionary;
        private readonly ILogger<Game> _logger;

        private List<Player> _players = new();
        int _currentPlayer = -1;
        string _currentCombination = string.Empty;
        System.Timers.Timer _timeoutTimer = new();

        public Game(ILogger<Game> logger)
        {
            _dictionary = new WordDictionary("english.txt");
            _logger = logger;

            _timeoutTimer.Elapsed += new ElapsedEventHandler(OnRoundTimeout);
        }

        public event Action<string, string>? OnRoundStart;
        public event Action? OnGameOver;
        public event Action<string, int>? OnHealthChanged;

        public bool Started { get; set; }
        public Player CurrentPlayer => _players[_currentPlayer];
        public IEnumerable<Player> Players => _players;
        public int StartHealthPoints { get; set; } = 5;
        public int RoundTime { get; set; } = 10000;

        public void Start()
        {
            Started = true;

            _currentPlayer = -1;
            _timeoutTimer.Interval = RoundTime;
            _timeoutTimer.Enabled = true;

            foreach (var player in _players)
            {
                player.HealthPoints = StartHealthPoints;
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

            if (_players.Count == 0 || _players.All(p => !p.Alive))
            {
                End();
                return;
            }

            _timeoutTimer.Start();

            NextPlayer();
            _currentCombination = _dictionary.GetRandomCombination();

            _logger.LogInformation("New round {}", _currentCombination);

            OnRoundStart?.Invoke(_players[_currentPlayer].Id, _currentCombination);
        }

        public bool SubmitAnswer(string userId, string answer)
        {
            var player = _players[_currentPlayer];
            if (player.Id != userId)
                return false;

            var right = answer.Contains(_currentCombination) && _dictionary.Contains(answer);
            if (right)
                _logger.LogInformation("{} has submitted the right answer: {}", player.DisplayName, answer);
            else
                _logger.LogInformation("{} has submitted the wrong answer: {}", player.DisplayName, answer);

            return right;
        }

        public void AddPlayer(string id)
        {
            _players.Add(new Player(id)
            {
                HealthPoints = StartHealthPoints
            });
        }

        public void RemovePlayer(string id)
        {
            var removeIndex = _players.FindIndex(p => p.Id == id);

            _players.RemoveAt(removeIndex);

            NextRound();
        }

        public void ChangePlayerName(string id, string newName)
        {
            _players.Single(p => p.Id == id).UserName = newName;
        }

        public void Dispose()
        {
            _timeoutTimer.Dispose();
        }
    }
}
