using BombParty.Commands;
using BombParty.Common;
using BombParty.Services;
using BombParty.ViewModels.Lobby;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _name;
        private Avatar _avatar;
        
        public SettingsViewModel(ISettingsStore settingsStore, IGameService gameService, 
            INavigationService<LobbyViewModel> lobbyNavService)
        {
            SubmitCommand = new ChangeSettingsCommand(this, settingsStore, gameService, lobbyNavService);

            _name = settingsStore.Settings.PlayerSettings.UserName ?? string.Empty;
            _avatar = Avatars.GetAvatar(settingsStore.Settings.PlayerSettings.AvatarId);
        }

        public ICommand SubmitCommand { get; }

        public IEnumerable<Avatar> AvailableAvatars => Avatars.AvailableAvatars;

        public string Name 
        { 
            get => _name; 
            set => SetField(ref _name, value); 
        }

        public Avatar Avatar 
        { 
            get => _avatar; 
            set => SetField(ref _avatar, value); 
        }
    }
}
