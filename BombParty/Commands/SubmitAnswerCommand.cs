using BombParty.Services;
using BombParty.ViewModels.Game;

namespace BombParty.Commands
{
    public class SubmitAnswerCommand : BaseCommand
    {
        private readonly IGameService _gameService;
        private readonly GameViewModel _gameViewModel;

        public SubmitAnswerCommand(GameViewModel gameViewModel, IGameService gameService)
        {
            _gameService = gameService;
            _gameViewModel = gameViewModel;

            _gameViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _gameService.SubmitInput(_gameViewModel.Input).ConfigureAwait(false);
            _gameViewModel.Input = "";
        }

        public override bool CanExecute(object? parameter)
        {
            return _gameViewModel.IsTurn;
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(GameViewModel.IsTurn))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
