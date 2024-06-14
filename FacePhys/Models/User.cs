using SQLite;

namespace FacePhys.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Gender Gender { get; set; }
    public int Age { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
}

public enum Gender
{
    Male,
    Female,
    Other,
}
