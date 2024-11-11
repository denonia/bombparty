using BombParty.Common;
using BombParty.Server.Hubs;
using BombParty.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace BombParty.Server.Services
{
    public class EventService : IEventService, IDisposable
    {
        private readonly IHubContext<GameHub, IGameServer> _hubContext;
        private readonly ILogger<EventService> _logger;
        private readonly List<Room> _rooms = new();

        public EventService(IHubContext<GameHub, IGameServer> hubContext, ILogger<EventService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public void RegisterRoom(Room room)
        {
            room.OnRoundStart += OnRoundStart;
            room.OnGameOver += OnGameOver;
            room.OnHealthChanged += OnHealthChanged;

            _rooms.Add(room);

            _logger.LogInformation("Registered room {}", room.Name);
        }

        public void UnregisterRoom(Room room)
        {
            room.OnRoundStart -= OnRoundStart;
            room.OnGameOver -= OnGameOver;
            room.OnHealthChanged -= OnHealthChanged;

            _rooms.Remove(room);
            _logger.LogInformation("Unregistered room {}", room.Name);
        }

        public void UnregisterRoom(string roomId)
        {
            var room = _rooms.SingleOrDefault(r => r.Id.ToString() == roomId);
            if (room is not null)
                UnregisterRoom(room);
        }

        public void Dispose()
        {
            foreach (var room in _rooms)
                UnregisterRoom(room);
        }

        private void OnRoundStart(string groupName, string userId, string combination)
        {
            _hubContext.Clients.Group(groupName).RoundStart(userId, combination);
            _logger.LogInformation("{} -> RoundStart ({}, {})", groupName, userId, combination);
        }

        private void OnGameOver(string groupName)
        {
            _hubContext.Clients.Group(groupName).GameOver();
            _logger.LogInformation("{} -> GameOver", groupName);
        }

        private void OnHealthChanged(string groupName, string userId, int newHealth)
        {
            _hubContext.Clients.Group(groupName).UserHealthChanged(userId, newHealth);
            _logger.LogInformation("{} -> OnHealthChanged ({}, {})", groupName, userId, newHealth);
        }
    }
}
