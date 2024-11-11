using BombParty.Common;
using System.IO;
using System.Text.Json;

namespace BombParty.Services
{
    public class SettingsStore
    {
        private const string SettingsPath = "settings.json";

        private Settings _settings;

        public SettingsStore()
        {
            if (File.Exists(SettingsPath))
            {
                using var stream = File.OpenRead(SettingsPath);
                _settings = JsonSerializer.Deserialize<Settings>(stream);
            }
            else
            {
                CreateDefaultSettings();
            }
        }

        public Settings Settings 
        { 
            get => _settings;
            set
            {
                _settings = value;

                var data = JsonSerializer.Serialize(_settings);
                File.WriteAllText(SettingsPath, data);
            }
        }

        private void CreateDefaultSettings()
        {
            var settings = new Settings
            {
                ServerAddress = "https://localhost:7173"
            };

            Settings = settings;
        }
    }
}
