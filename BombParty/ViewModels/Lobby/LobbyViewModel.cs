using BombParty.Commands;
using BombParty.Services;
using BombParty.ViewModels.Game;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BombParty.ViewModels.Lobby
{
    public class LobbyViewModel : BaseViewModel, IDisposable
    {
        private readonly GameService _gameService;
        private readonly SynchronizationContext _synchronizationContext;

        public LobbyViewModel(GameService gameService,
            NavigationService<CreateRoomViewModel> createRoomNavService,
            NavigationService<SettingsViewModel> settingsNavService,
            NavigationService<GameViewModel> gameNavService,
            SynchronizationContext synchronizationContext)
        {
            _gameService = gameService;
            _synchronizationContext = synchronizationContext;
            CreateRoomCommand = new NavigateCommand<CreateRoomViewModel>(createRoomNavService);
            JoinRoomCommand = new JoinRoomCommand(this, gameService, gameNavService);
            SettingsCommand = new NavigateCommand<SettingsViewModel>(settingsNavService);

            gameService.RequestActiveRooms().ConfigureAwait(false);
            gameService.OnActiveRooms += OnActiveRoomsReceived;
        }

        public ICommand CreateRoomCommand { get; }
        public JoinRoomCommand JoinRoomCommand { get; }
        public ICommand SettingsCommand { get; }

        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public bool LobbyIsEmpty => Rooms.Count == 0;

        private void OnActiveRoomsReceived(IList<Common.Dtos.RoomDetailsDto> roomDtos)
        {
            _synchronizationContext.Post((_) =>
            {
                Rooms.Clear();

                foreach (var roomDto in roomDtos)
                {
                    var roomViewModel = new RoomViewModel(roomDto.Id, roomDto.Name, roomDto.OwnerName, roomDto.PlayerNames.Length);
                    Rooms.Add(roomViewModel);
                }

                OnPropertyChanged(nameof(LobbyIsEmpty));
            }, null);
        }

        public override void Dispose()
        {
            _gameService.OnActiveRooms -= OnActiveRoomsReceived;

            JoinRoomCommand.Dispose();
        }
    }
}
