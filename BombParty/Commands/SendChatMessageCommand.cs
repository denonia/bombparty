using BombParty.Services;
using BombParty.ViewModels.Game;

namespace BombParty.Commands
{
    public class SendChatMessageCommand : BaseCommand
    {
        private readonly IGameService _gameService;
        private readonly GameViewModel _gameViewModel;

        public SendChatMessageCommand(GameViewModel gameViewModel, IGameService gameService)
        {
            _gameService = gameService;
            _gameViewModel = gameViewModel;
        }

        public override void Execute(object? parameter)
        {
            if (!string.IsNullOrEmpty(_gameViewModel.ChatInput))
            {
                _gameService.SendChatMessage(_gameViewModel.ChatInput).ConfigureAwait(false);
                _gameViewModel.ChatInput = "";
            }
        }
    }
}
