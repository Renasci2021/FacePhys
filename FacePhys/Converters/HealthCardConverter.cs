using System;
using System.Globalization;
using FacePhys.Models;


namespace FacePhys.Converters;

public class HealthCardImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var type = value;

        return type switch
        {
            HeartRate => "heart_rate.png",
            BloodPressure => "blood_pressure.png",
            BloodOxygen => "blood_oxygen.png",
            RespiratoryRate => "respiratoty_rate.png",
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
        var type = value;

        return type switch
        {
            HeartRate => "心率",
            BloodPressure => "血压",
            BloodOxygen => "血氧",
            RespiratoryRate => "呼吸率",
            _ => "",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
