using BombParty.ViewModels;
using BombParty.Views;
using System.Windows;

namespace BombParty.Services
{
    public class PasswordEntryService
    {
        public string PromptPassword()
        {
            var viewModel = new PasswordEntryViewModel();
            var window = new PasswordEntryWindow
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();

            return viewModel.Password;
        }
    }
}
