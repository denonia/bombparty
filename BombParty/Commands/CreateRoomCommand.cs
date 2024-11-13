using BombParty.Common;
using BombParty.Common.Dtos;
using BombParty.Services;
using BombParty.ViewModels;
using BombParty.ViewModels.Game;
using System.Windows;

namespace BombParty.Commands
{
    public class CreateRoomCommand : BaseCommand, IDisposable
    {
        private readonly CreateRoomViewModel _viewModel;
        private readonly GameService _gameService;
        private readonly NavigationService<GameViewModel> _gameNavService;

        public CreateRoomCommand(CreateRoomViewModel viewModel, GameService gameService, 
            NavigationService<GameViewModel> gameNavService)
        {
            _viewModel = viewModel;
            _gameService = gameService;
            _gameNavService = gameNavService;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            var dto = new CreateRoomDto(_viewModel.Name)
            {
                Password = _viewModel.Password,
                Settings = new RoomSettings
                {
                    Language = _viewModel.Dictionary.Language,
                    StartHealthPoints = _viewModel.StartHealthPoints,
                    RoundTime = _viewModel.RoundTime
                }
            };

            _gameService.OnJoinRoomResult += OnReceiveJoinResult;
            _gameService.CreateRoom(dto).Wait();
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) && !_viewModel.HasErrors;
        }

        private void OnReceiveJoinResult(bool success)
        {
            if (success)
                _gameNavService.Navigate();
            else
                MessageBox.Show("Failed to create room.", "Error");
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(CreateRoomViewModel.Name)
                or nameof(CreateRoomViewModel.StartHealthPoints)
                or nameof(CreateRoomViewModel.RoundTime))
            {
                OnCanExecuteChanged();
            }
        }

        public void Dispose()
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

            _gameService.OnJoinRoomResult -= OnReceiveJoinResult;
        }
    }
}
