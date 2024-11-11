namespace BombParty.ViewModels
{
    public class PasswordEntryViewModel : BaseViewModel
    {
        private string _password;

        public string Password 
        { 
            get => _password; 
            set => SetField(ref _password, value); 
        }
    }
}
