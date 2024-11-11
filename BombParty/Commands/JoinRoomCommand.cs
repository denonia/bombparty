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
        private readonly PasswordEntryService _passwordEntryService;
        private readonly NavigationService<GameViewModel> _gameNavService;

        public JoinRoomCommand(LobbyViewModel viewModel, GameService gameService, 
            PasswordEntryService passwordEntryService,
            NavigationService<GameViewModel> gameNavService)
        {
            _viewModel = viewModel;
            _gameService = gameService;
            _passwordEntryService = passwordEntryService;
            _gameNavService = gameNavService;
        }

        public override void Execute(object? parameter)
        {
            _gameService.OnJoinRoomResult += OnReceiveJoinResult;

            var roomId = (parameter as string)!;
            var roomViewModel = _viewModel.Rooms.Single(r => r.Id == roomId);

            string? password = null;

            if (roomViewModel.RequiresPassword)
                password = _passwordEntryService.PromptPassword();

            _gameService.JoinRoom(roomId, password).Wait();
        }

        private void OnReceiveJoinResult(bool success)
        {
            if (success)
                _gameNavService.Navigate();
            else
                MessageBox.Show("Invalid password.", "Error");

            _gameService.OnJoinRoomResult -= OnReceiveJoinResult;
        }

        public void Dispose()
        {
            _gameService.OnJoinRoomResult -= OnReceiveJoinResult;
        }
    }
}
