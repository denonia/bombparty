using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BombParty.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    internal class BrushDarkenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                double multiplier = 0.75;

                if (parameter is string paramString && double.TryParse(paramString, out double parsedMultiplier))
                {
                    multiplier = Math.Clamp(parsedMultiplier, 0.0, 1.0);
                }

                var color = brush.Color;
                var newColor = Color.FromArgb(color.A,
                                      (byte)(color.R * multiplier),
                                      (byte)(color.G * multiplier),
                                      (byte)(color.B * multiplier));

                return new SolidColorBrush(newColor);
            }

            throw new InvalidOperationException("Value must be a Color.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
