using BombParty.Services;
using BombParty.ViewModels;
using System.Windows.Input;

namespace BombParty.Commands
{
    public class SendChatMessageCommand : BaseCommand
    {
        private readonly GameService _gameService;
        private readonly GameViewModel _gameViewModel;

        public SendChatMessageCommand(GameViewModel gameViewModel, GameService gameService)
        {
            _gameService = gameService;
            _gameViewModel = gameViewModel;
        }

        public override void Execute(object? parameter)
        {
            _gameService.SendChatMessage(_gameViewModel.ChatInput).ConfigureAwait(false);
            _gameViewModel.ChatInput = "";
        }
    }
}
