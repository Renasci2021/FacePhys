using System;
using System.Globalization;
using FacePhys.Models;

namespace FacePhys.Converters;

public class HealthCardImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var type = (HealthMetric.MetricType)value;

        return type switch
        {
            HealthMetric.MetricType.HeartRate => "heart_rate.png",
            HealthMetric.MetricType.BloodPressure => "blood_pressure.png",
            HealthMetric.MetricType.BloodOxygen => "blood_oxygen.png",
            HealthMetric.MetricType.RespirationRate => "respiratoty_rate.png",
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
        var type = (HealthMetric.MetricType)value;

        return type switch
        {
            HealthMetric.MetricType.HeartRate => "心率",
            HealthMetric.MetricType.BloodPressure => "血压",
            HealthMetric.MetricType.BloodOxygen => "血氧",
            HealthMetric.MetricType.RespirationRate => "呼吸率",
            _ => "",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
