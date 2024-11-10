namespace BombParty.Server
{
    public class WordDictionary
    {
        private string[] _words;

        public WordDictionary(string[] words)
        {
            _words = words;
        }

        public WordDictionary(string path)
        {
            _words = File.ReadAllLines(path);
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
