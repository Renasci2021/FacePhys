using SQLite;

namespace FacePhys.Models;

public abstract class HealthMetric
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public HealthMetric()
    {
        Timestamp = DateTime.UtcNow;
    }
}
