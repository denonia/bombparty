using BombParty.Common;
using System.IO;
using System.Text.Json;

namespace BombParty.Services
{
    public class SettingsStore : ISettingsStore
    {
        private string _settingsDir;
        private string _settingsPath;

        private Settings _settings;

        public SettingsStore()
        {
            _settingsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BombParty");

            _settingsPath = Path.Combine(_settingsDir,
#if DEBUG
                "settings-debug.json"
#elif RELEASE
                "settings.json"
#endif
                );

            if (!Directory.Exists(_settingsDir))
                Directory.CreateDirectory(_settingsDir);

            if (File.Exists(_settingsPath))
            {
                using var stream = File.OpenRead(_settingsPath);
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
                File.WriteAllText(_settingsPath, data);
            }
        }

        private void CreateDefaultSettings()
        {
            var settings = new Settings
            {
                PlayerSettings = new PlayerSettings(),

#if DEBUG
                ServerAddress = "https://localhost:7173"
#elif RELEASE
                ServerAddress = "https://party.allein.xyz"
#endif
            };

            Settings = settings;
        }
    }
}
