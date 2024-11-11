using BombParty.Common;

namespace BombParty.ViewModels.Game
{
    public class PlayerViewModel : BaseViewModel
    {
        private string _displayName;
        private Avatar _avatar;
        private string _input;
        private int _healthPoints;

        private bool _isTurn;
        private bool _isRed;
        private bool _isGreen;

        public PlayerViewModel(string id)
        {
            Id = id;
            _displayName = id;
        }

        public string Id { get; }

        public string DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }

        public Avatar Avatar
        {
            get => _avatar;
            set => SetField(ref _avatar, value);
        }

        public string Input
        {
            get => _input;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    SetField(ref _input, value);
            }
        }

        public int HealthPoints
        {
            get => _healthPoints;
            set => SetField(ref _healthPoints, value);
        }

        public bool IsTurn
        {
            get => _isTurn;
            set => SetField(ref _isTurn, value);
        }

        public bool IsRed
        {
            get => _isRed;
            set => SetField(ref _isRed, value);
        }

        public bool IsGreen
        {
            get => _isGreen;
            set => SetField(ref _isGreen, value);
        }

        public void ShowRed()
        {
            if (!IsRed)
            {
                IsRed = true;
                Task.Delay(1000).ContinueWith(_ => IsRed = false);
            }
        }

        public void ShowGreen()
        {
            if (!IsGreen)
            {
                IsGreen = true;
                Task.Delay(1000).ContinueWith(_ => IsGreen = false);
            }
        }
    }
}
