using BombParty.ViewModels;

namespace BombParty.Services
{
    public class NavigationStore : INavigationStore
    {
        private BaseViewModel? _currentViewModel;

        public NavigationStore()
        {
            
        }

        public event Action? CurrentViewModelChanged;

        public BaseViewModel? CurrentViewModel 
        { 
            get => _currentViewModel;
            set 
            {
                if (_currentViewModel is not null)
                    _currentViewModel.Dispose();

                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }
    }
}
