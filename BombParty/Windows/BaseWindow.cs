using BombParty.Helpers;
using BombParty.Services;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BombParty.Windows
{
    public class BaseWindow : Window
    {
        public static readonly DependencyProperty ShowSwitchThemeButtonProperty =
            DependencyProperty.Register("ShowSwitchThemeButton", typeof(bool), typeof(BaseWindow), new PropertyMetadata(false));

        private const int WM_SYSCOMMAND = 0x112;

        private readonly IThemeService? _themeService;

        private HwndSource _hwndSource;

        private enum ResizeDirection
        {
            Left = 61441,
            Right = 61442,
            Top = 61443,
            TopLeft = 61444,
            TopRight = 61445,
            Bottom = 61446,
            BottomLeft = 61447,
            BottomRight = 61448,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public BaseWindow()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            MaxHeight = SystemParameters.VirtualScreenHeight;
            MaxWidth = SystemParameters.VirtualScreenWidth;

            SourceInitialized += BaseWindow_SourceInitialized;
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

            var resizeRects = new List<Rectangle?> {
                    GetTemplateChild("ResizeN") as Rectangle,
                    GetTemplateChild("ResizeE") as Rectangle,
                    GetTemplateChild("ResizeS") as Rectangle,
                    GetTemplateChild("ResizeW") as Rectangle
            };

            foreach (var resizeRect in resizeRects)
            {
                if (resizeRect is not null)
                {
                    resizeRect.MouseEnter += DisplayResizeCursor;
                    resizeRect.MouseLeave += ResetCursor;
                    resizeRect.PreviewMouseLeftButtonDown += Resize;
                }
            }
        }

        private void BaseWindow_SourceInitialized(object? sender, EventArgs e)
        {
            _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
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
            _themeService?.SwitchTheme();
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)direction, IntPtr.Zero);
        }

        protected void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
        }

        protected void Resize(object sender, MouseButtonEventArgs e)
        {
            var clickedShape = sender as Shape;

            switch (clickedShape?.Name)
            {
                case "ResizeN":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "ResizeE":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "ResizeS":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "ResizeW":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "ResizeNW":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "ResizeNE":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "ResizeSE":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                case "ResizeSW":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                default:
                    break;
            }
        }

        protected void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            var clickedShape = sender as Shape;

            switch (clickedShape?.Name)
            {
                case "ResizeN":
                case "ResizeS":
                    Cursor = Cursors.SizeNS;
                    break;
                case "ResizeE":
                case "ResizeW":
                    Cursor = Cursors.SizeWE;
                    break;
                case "ResizeNW":
                case "ResizeSE":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeNE":
                case "ResizeSW":
                    Cursor = Cursors.SizeNESW;
                    break;
                default:
                    break;
            }
        }

        protected void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
