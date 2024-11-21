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

        private Uri ThemeUri(string name)
        {
            return new Uri(Path.Combine("Themes", name), UriKind.Relative);
        }

        private void ApplyTheme(string path)
        {
            _resources.MergedDictionaries.Clear();

            var commonDictionary = new ResourceDictionary
            {
                Source = ThemeUri("Common.xaml"),
            };

            var themeDictionary = new ResourceDictionary
            {
                Source = ThemeUri(path)
            };

            commonDictionary.MergedDictionaries.Add(themeDictionary);
            _resources.MergedDictionaries.Add(commonDictionary);
        }
    }
}
