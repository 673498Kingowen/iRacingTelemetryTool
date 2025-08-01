﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using iRacingSimulator.Drivers; // Make sure this is included

namespace IracingTelemetry.MVVM.Converters
{
    public class LicenseToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is License license)
            {
                Color color = license.BackgroundColor;
                return new SolidColorBrush(color);
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}