namespace BombParty.ViewModels.Lobby
{
    public class RoomViewModel : BaseViewModel
    {
        private string _id;
        private string _title;
        private string _ownerName;
        private int _playerCount;
        private bool _requiresPassword;

        public RoomViewModel(string id, string title, string ownerName, int playerCount, bool requiresPassword)
        {
            Id = id;
            Title = title;
            OwnerName = ownerName;
            PlayerCount = playerCount;
            RequiresPassword = requiresPassword;
        }

        public string Id 
        { 
            get => _id; 
            set => SetField(ref _id, value); 
        }

        public string Title 
        { 
            get => _title; 
            set => SetField(ref _title, value); 
        }

        public string OwnerName 
        { 
            get => _ownerName; 
            set => SetField(ref _ownerName, value); 
        }

        public int PlayerCount 
        { 
            get => _playerCount; 
            set => SetField(ref _playerCount, value); 
        }

        public bool RequiresPassword 
        { 
            get => _requiresPassword; 
            set => SetField(ref _requiresPassword, value); 
        }

        public string OwnerText => "Hosted by " + OwnerName;
        public string PlayerCountText => "Players: " + PlayerCount;
    }
}
