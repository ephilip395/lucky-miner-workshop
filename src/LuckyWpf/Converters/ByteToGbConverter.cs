﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Lucky.Converters {
    [ValueConversion(typeof(double), typeof(string))]
    public class ByteToGbConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            double d = System.Convert.ToDouble(value);
            return (d / LuckyKeyword.LongG).ToString("f1") + " GB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}