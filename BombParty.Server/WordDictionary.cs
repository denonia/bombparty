using BombParty.Common.Enums;
using System.Reflection;
using System.Resources;

namespace BombParty.Server
{
    public class WordDictionary
    {
        // TODO: load these only once
        private string[] _words;

        public WordDictionary(DictionaryLanguage language)
        {
            var rm = new ResourceManager("BombParty.Server.Properties.Resources", Assembly.GetExecutingAssembly());

            var dictionaryName = language switch
            { 
                DictionaryLanguage.English => "English",
                DictionaryLanguage.Ukrainian => "Ukrainian",

                _ => "English"
            };

            var dictionaryText = (string)rm.GetObject(dictionaryName)!;

            _words = dictionaryText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public bool Contains(string word) => _words.Contains(word);

        public string GetRandomWord(int minLength = 2)
        {
            while (true)
            {
                var word = _words[Random.Shared.Next(_words.Length)];
                if (word.Length >= minLength)
                    return word;
            }
        }

        public string GetRandomCombination()
        {
            while (true)
            {
                var combinationLength = Random.Shared.Next(2, 4);
                var word = GetRandomWord(combinationLength);
                var pos = Random.Shared.Next(word.Length - combinationLength);
                var combination = word.Substring(pos, combinationLength);

                if (_words.Count(w => w.Contains(combination)) >= 500)
                    return combination;
            }
        }
    }
}
