using System;
using System.Globalization;
using System.Windows.Data;
using TagGame.Common;
using TagGame.ViewModels;

namespace TagGame.Converters
{
    public class StatusToStringConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GameStatus status)
            {
                return status switch
                {
                    GameStatus.Play => "Пауза",
                    GameStatus.Pause => "Продолжить",
                    _ => ""
                };
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
