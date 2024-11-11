namespace BombParty.ViewModels.Lobby
{
    public class RoomViewModel : BaseViewModel
    {
        private string _id;
        private string _title;
        private string _ownerName;
        private int _playerCount;

        public RoomViewModel(string id, string title, string ownerName, int playerCount)
        {
            Id = id;
            Title = title;
            OwnerName = ownerName;
            PlayerCount = playerCount;
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

        public string OwnerText => "Hosted by " + OwnerName;
        public string PlayerCountText => "Players: " + PlayerCount;
    }
}
