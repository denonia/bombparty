using BombParty.ViewModels;

namespace BombParty.Services
{
    public interface INavigationStore
    {
        public event Action? CurrentViewModelChanged;

        public BaseViewModel? CurrentViewModel { get; set; }
    }
}
