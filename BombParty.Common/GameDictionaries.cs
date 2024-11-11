using BombParty.Common.Enums;

namespace BombParty
{
    public class GameDictionary
    {
        public GameDictionary(DictionaryLanguage value, string fileName, string displayName)
        {
            Language = value;
            FileName = fileName;
            DisplayName = displayName;
        }

        public DictionaryLanguage Language { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }

        public override string ToString() => DisplayName;
    }

    public static class GameDictionaries
    {
        public static IList<GameDictionary> Dictionaries = 
        [
            new GameDictionary(DictionaryLanguage.English, "English.txt.gz", "English"),
            new GameDictionary(DictionaryLanguage.Ukrainian, "Ukrainian.txt.gz", "Українська"),
            new GameDictionary(DictionaryLanguage.Polish, "Polish.txt.gz", "Polski"),
        ];
    }
}
