using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Calculator.ViewModels;
using static Calculator.ViewModels.MainViewModel;

namespace Calculator.Converters
{
    public class ProgrammerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isProgrammer && isProgrammer)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
