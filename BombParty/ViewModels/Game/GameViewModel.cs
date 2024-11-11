using BombParty.Commands;
using BombParty.Common;
using BombParty.Services;
using BombParty.ViewModels.Lobby;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace BombParty.ViewModels.Game
{
    public class GameViewModel : BaseViewModel
    {
        private readonly SynchronizationContext _synchronizationContext;
        private readonly GameService _gameService;
        private readonly AudioService _audioService;

        private bool _isTurn;
        private string _currentCombination;
        private string _input;
        private string _chatHistory;
        private string _chatInput;
        private DateTime _roundStartTime;
        private Timer _progressTimer;

        public GameViewModel(SynchronizationContext synchronizationContext, 
            GameService gameService, AudioService audioService,
            NavigationService<LobbyViewModel> lobbyNavService)
        {
            _synchronizationContext = synchronizationContext;
            _gameService = gameService;
            _audioService = audioService;
            _gameService.OnRoundStart += OnRoundStart;
            _gameService.OnGameOver += OnGameOver;
            _gameService.OnHealthChanged += OnHealthChanged;
            _gameService.OnChatMessage += OnChatMessage;
            _gameService.OnUserPresence += OnUserJoined;
            _gameService.OnUserJoined += OnUserJoined;
            _gameService.OnUserLeft += OnUserLeft;
            _gameService.OnUserTyping += OnUserTyping;
            _gameService.OnAnswerSubmitted += OnAnswerSubmitted;

            SubmitAnswerCommand = new SubmitAnswerCommand(this, _gameService);
            SendChatMessageCommand = new SendChatMessageCommand(this, _gameService);
            LeaveRoomCommand = new LeaveRoomCommand(_gameService, lobbyNavService);

            _progressTimer = new Timer(10);
            _progressTimer.Elapsed += OnProgressTimerTick;
            _progressTimer.Start();
        }

        public ICommand SubmitAnswerCommand { get; }
        public ICommand SendChatMessageCommand { get; }
        public ICommand LeaveRoomCommand { get; }

        public ObservableCollection<PlayerViewModel> Players { get; set; } = new();

        public int RoundProgress => 100 - (int)((DateTime.Now - _roundStartTime).TotalMilliseconds / (_gameService.RoomSettings.RoundTime * 1000.0) * 100.0);

        public bool IsTurn
        {
            get => _isTurn;
            set => SetField(ref _isTurn, value);
        }

        public string CurrentCombination
        {
            get => _currentCombination;
            set => SetField(ref _currentCombination, value);
        }

        public string Input
        {
            get => _input;
            set
            {
                SetField(ref _input, value);
                Players.Single(p => p.Id == _gameService.ConnectionId).Input = value;
                _gameService.SetInput(value).ConfigureAwait(false);
            }
        }

        public string ChatHistory
        {
            get => _chatHistory;
            set => SetField(ref _chatHistory, value);
        }

        public string ChatInput
        {
            get => _chatInput;
            set => SetField(ref _chatInput, value);
        }

        private void OnRoundStart(string userId, string currentCombination)
        {
            foreach (var player in Players)
                player.IsTurn = player.Id == userId;

            _synchronizationContext.Post((_) =>
            {
                IsTurn = userId == _gameService.ConnectionId;
            }, null);

            CurrentCombination = currentCombination;
            _roundStartTime = DateTime.Now;

            _audioService.Stop();
        }

        private void OnGameOver()
        {
            // No need to show the winner if we are the only player in lobby
            if (Players.Count(p => !p.IsDead) == 1)
            {
                var winner = Players.Single(p => !p.IsDead);

                WriteChatLine($"{winner.DisplayName} is the winner! King gg");

                _audioService.PlayGameOver();
            }
            else
                WriteChatLine($"Game over.");
        }

        private void OnHealthChanged(string userId, int newHealth)
        {
            Players.Single(p => p.Id == userId).HealthPoints = newHealth;
        }

        private void OnChatMessage(string senderName, string text)
        {
            WriteChatLine($"{senderName}: {text}");
        }

        private void OnUserJoined(Player player)
        {
            _synchronizationContext.Post((_) =>
            {
                Players.Add(new PlayerViewModel(player.Id)
                {
                    DisplayName = player.DisplayName,
                    Avatar = Avatars.GetAvatar(player.Settings.AvatarId),
                    HealthPoints = player.HealthPoints
                });
            }, null);
        }

        private void OnUserLeft(string userId)
        {
            _synchronizationContext.Post((_) =>
            {
                var user = Players.Single(p => p.Id == userId);
                Players.Remove(user);
            }, null);
        }

        private void OnUserNameChanged(string userId, string newName)
        {
            Players.Single(p => p.Id == userId).DisplayName = newName;
        }

        private void OnUserTyping(string userId, string input)
        {
            Players.Single(p => p.Id == userId).Input = input;
        }

        private void OnAnswerSubmitted(string userId, string input, bool right)
        {
            var player = Players.Single(p => p.Id == userId);

            if (right)
                player.ShowGreen();
            else
                player.ShowRed();
        }

        private void WriteChatLine(string text)
        {
            if (string.IsNullOrEmpty(ChatHistory))
                ChatHistory += $"{text}";
            else
                ChatHistory += $"{Environment.NewLine}{text}";
        }

        private void OnProgressTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(RoundProgress));
        }

        public override void Dispose()
        {
            _gameService.OnRoundStart -= OnRoundStart;
            _gameService.OnGameOver -= OnGameOver;
            _gameService.OnHealthChanged -= OnHealthChanged;
            _gameService.OnChatMessage -= OnChatMessage;
            _gameService.OnUserPresence -= OnUserJoined;
            _gameService.OnUserJoined -= OnUserJoined;
            _gameService.OnUserLeft -= OnUserLeft;
            _gameService.OnUserTyping -= OnUserTyping;
            _gameService.OnAnswerSubmitted -= OnAnswerSubmitted;

            _progressTimer.Elapsed -= OnProgressTimerTick;
        }
    }
}
