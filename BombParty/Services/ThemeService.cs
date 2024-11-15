using System.IO;
using System.Windows;

namespace BombParty.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ResourceDictionary _resources;
        private readonly ISettingsStore _settingsStore;

        public ThemeService(Application application, ISettingsStore settingsStore)
        {
            _resources = application.Resources;
            _settingsStore = settingsStore;

            if (_settingsStore.Settings.UseDarkTheme)
                ApplyDarkTheme();
            else
                ApplyLightTheme();
        }

        public void SwitchTheme()
        {
            if (_settingsStore.Settings.UseDarkTheme)
                ApplyLightTheme();
            else
                ApplyDarkTheme();
        }

        public void ApplyLightTheme()
        {
            ApplyTheme("Light.xaml");
            _settingsStore.Settings = _settingsStore.Settings with { UseDarkTheme = false };
        }

        public void ApplyDarkTheme()
        {
            ApplyTheme("Dark.xaml");
            _settingsStore.Settings = _settingsStore.Settings with { UseDarkTheme = true };
        }

        private void ApplyTheme(string path)
        {
            _resources.MergedDictionaries.Clear();

            var dictionary = new ResourceDictionary
            {
                Source = new Uri(Path.Combine("Themes", path), UriKind.Relative)
            };

            _resources.MergedDictionaries.Add(dictionary);
        }
    }
}
