using BombParty.ViewModels;

namespace BombParty.Services
{
    public class NavigationService<TViewModel> : INavigationService<TViewModel> 
        where TViewModel : BaseViewModel
    {
        private readonly INavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(INavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
