using BombParty.Server.Models;

namespace BombParty.Server.Services
{
    public interface IEventService
    {
        public void RegisterRoom(Room room);
        public void UnregisterRoom(string roomId);
    }
}
