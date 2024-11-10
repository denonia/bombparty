using System.Windows.Controls;

namespace BombParty.Controls
{
    public class ChatTextBox : TextBox
    {
        public bool AtBottom => VerticalOffset + ViewportHeight >= ExtentHeight - 1;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IsReadOnly = true;
            TextWrapping = System.Windows.TextWrapping.Wrap;
            AcceptsReturn = true;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (AtBottom)
            {
                CaretIndex = Text.Length;
                ScrollToEnd();
            }
        }
    }
}
