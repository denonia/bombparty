namespace BombParty.Common.Dtos
{
    public class RoomDetailsDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string[] PlayerNames { get; set; }
        public RoomSettings Settings { get; set; }
    }
}
