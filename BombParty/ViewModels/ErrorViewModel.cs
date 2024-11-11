using BombParty.Commands;
using System.Windows;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class ErrorViewModel : BaseViewModel
    {
        public ErrorViewModel(string message)
        {
            Message = message;

            CopyCommand = new RelayCommand((_) =>
            {
                Clipboard.SetText(message);
            });
        }

        public string Message { get; }
        public ICommand CopyCommand { get; }
    }
}
