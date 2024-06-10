using System;
using System.Globalization;
using FacePhys.Models;


namespace FacePhys.Converters;

public class HealthCardImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var type = (MetricType)value;

        return type switch
        {
            MetricType.HeartRate => "heart_rate.png",
            MetricType.BloodPressure => "blood_pressure.png",
            MetricType.BloodOxygen => "blood_oxygen.png",
            MetricType.RespiratoryRate => "respiratoty_rate.png",
            _ => "",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HealthCardTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var type = (MetricType)value;

        return type switch
        {
            MetricType.HeartRate => "心率",
            MetricType.BloodPressure => "血压",
            MetricType.BloodOxygen => "血氧",
            MetricType.RespiratoryRate => "呼吸率",
            _ => "",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
