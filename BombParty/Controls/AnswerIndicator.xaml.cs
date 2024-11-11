using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BombParty.Controls
{
    /// <summary>
    /// Interaction logic for AnswerIndicator.xaml
    /// </summary>
    public partial class AnswerIndicator : UserControl
    {
        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(AnswerIndicator), new PropertyMetadata(false));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(AnswerIndicator), 
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));

        public AnswerIndicator()
        {
            InitializeComponent();
        }

        public bool IsShown
        {
            get => (bool)GetValue(IsShownProperty);
            set => SetValue(IsShownProperty, value);
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }
    }
}
