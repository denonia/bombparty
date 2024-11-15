
using BombParty.Services;
using BombParty.ViewModels.Lobby;

namespace BombParty.Commands
{
    public class LeaveRoomCommand : BaseCommand
    {
        private readonly IGameService _gameService;
        private readonly INavigationService<LobbyViewModel> _lobbyNavService;

        public LeaveRoomCommand(IGameService gameService, 
            INavigationService<LobbyViewModel> lobbyNavService)
        {
            _gameService = gameService;
            _lobbyNavService = lobbyNavService;
        }

        public override void Execute(object? parameter)
        {
            Task.Run(_gameService.LeaveRoom);

            _lobbyNavService.Navigate();
        }
    }
}
