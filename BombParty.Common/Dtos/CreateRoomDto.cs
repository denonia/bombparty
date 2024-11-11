namespace BombParty.Common.Dtos
{
    public class CreateRoomDto
    {
        public CreateRoomDto(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string? Password { get; set; }

        public RoomSettings Settings { get; set; }
    }
}
