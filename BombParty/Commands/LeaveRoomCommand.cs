
using BombParty.Services;
using BombParty.ViewModels.Lobby;

namespace BombParty.Commands
{
    public class LeaveRoomCommand : BaseCommand
    {
        private readonly GameService _gameService;
        private readonly NavigationService<LobbyViewModel> _lobbyNavService;

        public LeaveRoomCommand(GameService gameService, 
            NavigationService<LobbyViewModel> lobbyNavService)
        {
            _gameService = gameService;
            _lobbyNavService = lobbyNavService;
        }

        public override void Execute(object? parameter)
        {
            _gameService.LeaveRoom().ConfigureAwait(false);

            _lobbyNavService.Navigate();
        }
    }
}
