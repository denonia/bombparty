using BombParty.Helpers;
using BombParty.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BombParty.Windows
{
    public class BaseWindow : Window
    {
        public static readonly DependencyProperty ShowSwitchThemeButtonProperty =
            DependencyProperty.Register("ShowSwitchThemeButton", typeof(bool), typeof(BaseWindow), new PropertyMetadata(false));

        private readonly IThemeService? _themeService;

        public BaseWindow()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            MaxHeight = SystemParameters.VirtualScreenHeight;
            MaxWidth = SystemParameters.VirtualScreenWidth;

            Loaded += BaseWindow_Loaded;
        }

        public BaseWindow(IThemeService themeService) : this()
        {
            _themeService = themeService;
        }

        public bool ShowSwitchThemeButton
        {
            get { return (bool)GetValue(ShowSwitchThemeButtonProperty); }
            set { SetValue(ShowSwitchThemeButtonProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var titlePanel = GetTemplateChild("TitlePanel") as Panel;
            var closeButton = GetTemplateChild("CloseButton") as Button;
            var minimizeButton = GetTemplateChild("MinimizeButton") as Button;
            var zoomButton = GetTemplateChild("ZoomButton") as Button;
            var switchThemeButton = GetTemplateChild("SwitchThemeButton") as Button;

            if (titlePanel is not null)
                titlePanel.MouseDown += TitlePanel_MouseDown;

            if (closeButton is not null)
                closeButton.Click += CloseButton_Click;
            if (minimizeButton is not null)
                minimizeButton.Click += MinimizeButton_Click;
            if (zoomButton is not null)
                zoomButton.Click += ZoomButton_Click;

            if (switchThemeButton is not null && _themeService is not null)
            {
                switchThemeButton.Click += SwitchThemeButton_Click;
                ShowSwitchThemeButton = true;
            }
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowBlurHelper.EnableBlur(this);
        }

        private void TitlePanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void SwitchThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _themeService.SwitchTheme();
        }
    }
}
