namespace FacePhys.Models;

public abstract class HealthMetric
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public MetricType Type { get; protected set; }

    public enum MetricType
    {
        BloodPressure,
        BloodOxygen,
        HeartRate,
        RespirationRate
    }
}

public class HeartRate : HealthMetric
{
    public int Value { get; set; }

    public HeartRate()
    {
        Type = MetricType.HeartRate;
    }

    public HeartRate(int value)
    {
        Type = MetricType.HeartRate;
        Value = value;
    }
}

public class BloodPressure : HealthMetric
{
    public int Systolic { get; set; }
    public int Diastolic { get; set; }

    public BloodPressure()
    {
        Type = MetricType.BloodPressure;
    }

    public BloodPressure(int diastolic, int systolic)
    {
        Type = MetricType.BloodPressure;
        Systolic = systolic;
        Diastolic = diastolic;//舒张压 数值小
    }
}

public class BloodOxygen : HealthMetric
{
    public float Saturation { get; set; }

    public BloodOxygen()
    {
        Type = MetricType.BloodOxygen;
    }

    public BloodOxygen(float saturation)
    {
        Type = MetricType.BloodOxygen;
        Saturation = saturation;
    }
}

public class RespirationRate : HealthMetric
{
    public int Value { get; set; }

    public RespirationRate()
    {
        Type = MetricType.RespirationRate;
    }

    public RespirationRate(int value)
    {
        Type = MetricType.RespirationRate;
        Value = value;
    }
}
