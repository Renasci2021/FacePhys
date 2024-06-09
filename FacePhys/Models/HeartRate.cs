namespace FacePhys.Models;

public class HeartRate : HealthMetric
{
    public float BeatsPerMinute { get; set; }

    public HeartRate(float beatsPerMinute)
    {
        Type = MetricType.HeartRate;
        BeatsPerMinute = beatsPerMinute;
    }
}
