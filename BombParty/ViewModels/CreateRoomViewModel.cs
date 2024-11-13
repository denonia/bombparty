using BombParty.Commands;
using BombParty.Services;
using BombParty.ViewModels.Game;
using BombParty.ViewModels.Lobby;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class CreateRoomViewModel : BaseViewModel, INotifyDataErrorInfo, IDisposable
    {
        private readonly Dictionary<string, List<string>> _propertyErrors = new();

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

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _propertyErrors.Count != 0;

        public ICommand BackCommand { get; }
        public CreateRoomCommand SubmitCommand { get; }

        public IEnumerable<GameDictionary> Dictionaries => GameDictionaries.Dictionaries;

        public string Name 
        {
            get => _name;
            set
            {
                _propertyErrors.Remove(nameof(Name));
                if (string.IsNullOrEmpty(value))
                {
                    _propertyErrors.Add(nameof(Name), ["Room name can't be empty."]);
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
                }

                SetField(ref _name, value);
            }
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
            set  
            {
                _propertyErrors.Remove(nameof(StartHealthPoints));
                if (value <= 0)
                {
                    _propertyErrors.Add(nameof(StartHealthPoints), ["Start health points can't be negative."]);
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(StartHealthPoints)));
                }

                SetField(ref _startHealthPoints, value);
            }
        }

        public int RoundTime 
        {
            get => _roundtime;
            set
            {
                _propertyErrors.Remove(nameof(RoundTime));
                if (value <= 0)
                {
                    _propertyErrors.Add(nameof(RoundTime), ["Round time can't be negative."]);
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(RoundTime)));
                }

                SetField(ref _roundtime, value);
            }
        }

        public override void Dispose()
        {
            SubmitCommand.Dispose();
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            return _propertyErrors!.GetValueOrDefault(propertyName, []);
        }
    }
}
