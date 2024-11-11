using BombParty.Services;
using BombParty.ViewModels.Game;
using BombParty.ViewModels.Lobby;
using System.Windows;

namespace BombParty.Commands
{
    public class JoinRoomCommand : BaseCommand, IDisposable
    {
        private readonly LobbyViewModel _viewModel;
        private readonly GameService _gameService;
        private readonly NavigationService<GameViewModel> _gameNavService;

        public JoinRoomCommand(LobbyViewModel viewModel, GameService gameService, 
            NavigationService<GameViewModel> gameNavService)
        {
            _viewModel = viewModel;
            _gameService = gameService;
            _gameNavService = gameNavService;
        }

        public override void Execute(object? parameter)
        {
            _gameService.OnJoinRoomResult += OnReceiveJoinResult;

            var roomId = parameter as string;
            _gameService.JoinRoom(roomId, "").Wait();
        }

        private void OnReceiveJoinResult(bool success)
        {
            if (success)
                _gameNavService.Navigate();
            else
                MessageBox.Show("Invalid password.", "Error");
        }

        public void Dispose()
        {
            _gameService.OnJoinRoomResult -= OnReceiveJoinResult;
        }
    }
}
