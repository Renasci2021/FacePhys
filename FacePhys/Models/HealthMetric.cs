using SQLite;

namespace FacePhys.Models;

public class HealthMetric
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public MetricType Type { get; protected set; }

    [Indexed]
    public int UserId { get; set; }

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
