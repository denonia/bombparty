using BombParty.Common.Enums;

namespace BombParty.Common
{
    public struct RoomSettings
    {
        public RoomSettings()
        {
        }

        public DictionaryLanguage Language { get; set; } = DictionaryLanguage.English;
        public int StartHealthPoints { get; set; } = 5;
        public int RoundTime { get; set; } = 5;
    }
}
