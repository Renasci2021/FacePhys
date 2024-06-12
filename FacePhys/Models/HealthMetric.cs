using System.ComponentModel;
using SQLite;

namespace FacePhys.Models;

public abstract class HealthMetric
{
    public event PropertyChangedEventHandler PropertyChanged;
     
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
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
