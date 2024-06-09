using SQLite;
using Xamarin.Google.ErrorProne.Annotations;

namespace FacePhys.Models;

public class HealthMetric
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public MetricType Type { get; set; }

    [Indexed]
    public int UserId { get; set; }

    [Ignore]
    public float HeartRate
    {
        get => _value1;
        set => _value1 = value;
    }

    [Ignore]
    public Tuple<float, float> BloodPressure
    {
        get => new(_value1, _value2);
        set
        {
            _value1 = value.Item1;
            _value2 = value.Item2;
        }
    }

    [Ignore]
    public float BloodOxygen
    {
        get => _value1;
        set => _value1 = value;
    }

    [Ignore]
    public float RespiratoryRate
    {
        get => _value1;
        set => _value1 = value;
    }

    private float _value1;
    private float _value2;

    public HealthMetric()
    {
        Timestamp = DateTime.UtcNow;
    }
}

public enum MetricType
{
    BloodOxygen,
    BloodPressure,
    HeartRate,
    RespiratoryRate,
}
