using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Lucky.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    class ConsoleColorToControllColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConsoleColor src = (ConsoleColor)value;
            switch (src)
            {
                case ConsoleColor.Black:
                    return Brushes.Black;
                case ConsoleColor.DarkBlue:
                    return Brushes.DarkBlue;
                case ConsoleColor.DarkGreen:
                    return Brushes.DarkGreen;
                case ConsoleColor.DarkCyan:
                    return Brushes.DarkCyan;
                case ConsoleColor.DarkRed:
                    return Brushes.DarkRed;
                case ConsoleColor.DarkMagenta:
                    return Brushes.DarkMagenta;
                case ConsoleColor.DarkYellow:
                    return Brushes.GreenYellow;
                case ConsoleColor.Gray:
                    return Brushes.Gray;
                case ConsoleColor.DarkGray:
                    return Brushes.DarkGray;
                case ConsoleColor.Blue:
                    return Brushes.Blue;
                case ConsoleColor.Green:
                    return Brushes.Green;
                case ConsoleColor.Cyan:
                    return Brushes.Cyan;
                case ConsoleColor.Red:
                    return Brushes.Red;
                case ConsoleColor.Magenta:
                    return Brushes.Magenta;
                case ConsoleColor.Yellow:
                    return Brushes.Yellow;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
