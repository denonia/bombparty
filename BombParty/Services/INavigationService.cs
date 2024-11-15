using BombParty.ViewModels;

namespace BombParty.Services
{
    public interface INavigationService<TViewModel> where TViewModel : BaseViewModel
    {
        public void Navigate();
    }
}
