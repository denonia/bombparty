using System.Windows;
using BombParty.Helpers;

namespace BombParty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowBlurHelper.EnableBlur(this);
        }
    }
}