using BombParty.Common.Enums;

namespace BombParty.Server.Services
{
    public interface IDictionaryService
    {
        WordDictionary GetDictionary(DictionaryLanguage language);
    }
}
