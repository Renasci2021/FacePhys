using SQLite;
using System.ComponentModel;
namespace FacePhys.Models;

public class User
{
    string _avatarUrl = string.Empty;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl
    {
        get
        {
            if (string.IsNullOrEmpty(_avatarUrl))
            {
                return "Resources/Images/default_avatar.png";
            }
            return _avatarUrl;
        }
        set
        {
            _avatarUrl = value;
        }
    }
    public string Password { get; set; } = string.Empty;

    public Gender Gender { get; set; }
    public int Age { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
}

public enum Gender
{
    [Description("男")]
    Male,
    [Description("女")]
    Female,
    [Description("其他")]
    Other,
}
