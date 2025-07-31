using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IracingTelemetry.MVVM.Converters
{
    public class LicenseToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string licString || string.IsNullOrEmpty(licString))
                return Brushes.White;
            
            if (licString.StartsWith("R") || licString.StartsWith("Rookie"))
                return new SolidColorBrush(Color.FromRgb(255, 0, 0)); // Red for Rookie

            if (licString.StartsWith("D"))
                return new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange for D

            if (licString.StartsWith("C"))
                return new SolidColorBrush(Color.FromRgb(255, 255, 0)); // Yellow for C

            if (licString.StartsWith("B"))
                return new SolidColorBrush(Color.FromRgb(0, 255, 0)); // Green for B

            if (licString.StartsWith("A"))
                return new SolidColorBrush(Color.FromRgb(0, 0, 255)); // Blue for A

            if (licString.StartsWith("P") || licString.Contains("Pro"))
                return new SolidColorBrush(Color.FromRgb(0, 0, 0)); // Black for Pro

            return Brushes.White; // Default
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}