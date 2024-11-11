namespace BombParty.Common
{
    public struct RoomSettings
    {
        public RoomSettings()
        {
        }

        public int StartHealthPoints { get; set; } = 5;
        public int RoundTime { get; set; } = 5;
    }
}
