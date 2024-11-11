using BombParty.Common.Enums;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace BombParty.Server.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly Dictionary<DictionaryLanguage, WordDictionary> _loadedDictionaries = new();
        private readonly ILogger<DictionaryService> _logger;

        public DictionaryService(ILogger<DictionaryService> logger)
        {
            _logger = logger;

            LoadDictionaries();
        }

        public WordDictionary GetDictionary(DictionaryLanguage language)
        {
            return _loadedDictionaries[language];
        }

        private void LoadDictionaries()
        {
            var watch = Stopwatch.StartNew();

            foreach (var dict in GameDictionaries.Dictionaries)
            {
                LoadDictionary(dict.Language, dict.FileName);
            }

            watch.Stop();
            _logger.LogInformation("Loaded {} dictionaries in {} ms", _loadedDictionaries.Count, watch.ElapsedMilliseconds);
        }

        private void LoadDictionary(DictionaryLanguage language, string fileName)
        {
            var path = $"BombParty.Server.Resources.{fileName}";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            using var gs = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(gs, System.Text.Encoding.UTF8);

            var dictionaryText = reader.ReadToEnd();
            var words = dictionaryText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            _loadedDictionaries[language] = new WordDictionary(words);
        }
    }
}
