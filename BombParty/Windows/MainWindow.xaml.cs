using System.Windows;
using BombParty.Helpers;
using BombParty.Services;

namespace BombParty.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        public MainWindow(IThemeService themeService) : base(themeService)
        {
            InitializeComponent();
        }
    }
}