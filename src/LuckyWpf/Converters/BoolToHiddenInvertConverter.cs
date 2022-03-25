﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lucky.Converters {
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToHiddenInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
