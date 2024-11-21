using System.IO;
using System.Windows;

namespace BombParty.Services
{
    public class ThemeService : IThemeService
    {
        private static Uri LightThemeUri = ThemeUri("Light.xaml");
        private static Uri DarkThemeUri = ThemeUri("Dark.xaml");

        private static Uri[] CommonThemeUris = [
            ThemeUri("Common.xaml"),
            ThemeUri("Window.xaml"),
        ];

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
            ApplyTheme(LightThemeUri);
            _settingsStore.Settings = _settingsStore.Settings with { UseDarkTheme = false };
        }

        public void ApplyDarkTheme()
        {
            ApplyTheme(DarkThemeUri);
            _settingsStore.Settings = _settingsStore.Settings with { UseDarkTheme = true };
        }

        private static Uri ThemeUri(string name)
        {
            return new Uri(Path.Combine("Themes", name), UriKind.Relative);
        }

        private void ApplyTheme(Uri themeUri)
        {
            _resources.MergedDictionaries.Clear();

            foreach (var uri in CommonThemeUris)
            {
                var dictionary = new ResourceDictionary
                {
                    Source = uri
                };

                _resources.MergedDictionaries.Add(dictionary);
            }

            var themeDictionary = new ResourceDictionary
            {
                Source = themeUri
            };

            _resources.MergedDictionaries.Add(themeDictionary);
        }
    }
}
