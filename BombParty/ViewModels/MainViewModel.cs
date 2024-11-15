using BombParty.Commands;
using BombParty.Services;
using System.Windows.Input;

namespace BombParty.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationStore _navigationStore;
        private readonly IThemeService _themeService;

        public MainViewModel(INavigationStore navigationStore, IThemeService themeService)
        {
            _navigationStore = navigationStore;
            _themeService = themeService;

            SwitchThemeCommand = new RelayCommand((_) =>
            {
                themeService.SwitchTheme();
            });

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        public ICommand SwitchThemeCommand { get; }

        public BaseViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
