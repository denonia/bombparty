using BombParty.Commands;
using BombParty.Services;
using BombParty.ViewModels.Game;
using BombParty.ViewModels.Lobby;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class CreateRoomViewModel : BaseViewModel, IDisposable
    {
        private readonly SettingsStore _settingsStore;
        private string _name;
        private string _password = string.Empty;
        private GameDictionary _dictionary = GameDictionaries.Dictionaries.First();
        private int _startHealthPoints = 5;
        private int _roundtime = 5;

        public CreateRoomViewModel(GameService gameService, 
            SettingsStore settingsStore,
            NavigationService<LobbyViewModel> lobbyNavService,
            NavigationService<GameViewModel> gameNavService)
        {
            _settingsStore = settingsStore;
            _name = (settingsStore.Settings.PlayerSettings.UserName ?? gameService.ConnectionId) + "'s game";

            BackCommand = new NavigateCommand<LobbyViewModel>(lobbyNavService);
            SubmitCommand = new CreateRoomCommand(this, gameService, gameNavService);
        }

        public ICommand BackCommand { get; }
        public CreateRoomCommand SubmitCommand { get; }

        public IEnumerable<GameDictionary> Dictionaries => GameDictionaries.Dictionaries;

        public string Name 
        {
            get => _name; 
            set => SetField(ref _name, value); 
        }

        public string Password 
        {
            get => _password; 
            set => SetField(ref _password, value); 
        }

        public GameDictionary Dictionary 
        {
            get => _dictionary; 
            set => SetField(ref _dictionary, value); 
        }

        public int StartHealthPoints 
        {
            get => _startHealthPoints; 
            set => SetField(ref _startHealthPoints, value); 
        }

        public int RoundTime 
        {
            get => _roundtime; 
            set => SetField(ref _roundtime, value); 
        }

        public override void Dispose()
        {
            SubmitCommand.Dispose();
        }
    }
}
