using BombParty.Commands;
using BombParty.Services;
using BombParty.ViewModels.Lobby;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _name;
        
        public SettingsViewModel(SettingsStore settingsStore, GameService gameService, 
            NavigationService<LobbyViewModel> lobbyNavService)
        {
            SubmitCommand = new ChangeSettingsCommand(this, settingsStore, gameService, lobbyNavService);

            _name = settingsStore.Settings.PlayerSettings.UserName ?? string.Empty;
        }

        public ICommand SubmitCommand { get; }

        public string Name 
        { 
            get => _name; 
            set => SetField(ref _name, value); 
        }
    }
}
