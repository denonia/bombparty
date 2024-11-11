using BombParty.Common;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Lobby;

namespace BombParty.Commands
{
    public class ChangeSettingsCommand : BaseCommand
    {
        private readonly SettingsViewModel _viewModel;
        private readonly SettingsStore _settingsStore;
        private readonly GameService _gameService;
        private readonly NavigationService<LobbyViewModel> _lobbyNavService;

        public ChangeSettingsCommand(SettingsViewModel viewModel, SettingsStore settingsStore,
            GameService gameService, NavigationService<LobbyViewModel> lobbyNavService)
        {
            _viewModel = viewModel;
            _settingsStore = settingsStore;
            _gameService = gameService;
            _lobbyNavService = lobbyNavService;
        }

        public override void Execute(object? parameter)
        {
            var newPlayerSettings = new PlayerSettings
            {
                UserName = _viewModel.Name,
                AvatarId = _viewModel.Avatar.Id
            };

            var newSettings = _settingsStore.Settings;
            newSettings.PlayerSettings = newPlayerSettings;
            _settingsStore.Settings = newSettings;

            _gameService.UpdateSettings(newPlayerSettings).ConfigureAwait(false);
            _lobbyNavService.Navigate();
        }
    }
}
