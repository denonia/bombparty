using BombParty.Common.Enums;

namespace BombParty
{
    public static class Languages
    {
        public class Language
        {
            public Language(DictionaryLanguage value, string displayName)
            {
                Value = value;
                DisplayName = displayName;
            }

            public DictionaryLanguage Value { get; set; }
            public string DisplayName { get; set; }

            public override string ToString() => DisplayName;
        }

        public static IList<Language> DictionaryLanguages = 
        [
            new Language(DictionaryLanguage.English, "English"),
            new Language(DictionaryLanguage.Ukrainian, "Українська"),
        ];
    }
}
