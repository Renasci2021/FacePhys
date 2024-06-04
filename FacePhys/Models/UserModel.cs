namespace FacePhys.Models;

/// <summary>
/// 用户模型类，用于表示一个用户
/// </summary>
public class UserModel
{
    /// <summary>
    /// 用一标识符
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }
    public string UserID { get; set; }
    public string Gender { get; set; }
    public string Age { get; set; }
    public string Height { get; set; }
    public string Weight { get; set; }
    /// <summary>
    /// 头像 url
    /// </summary>
    public string AvatarUrl { get; set; }

    /// <summary>
    /// 个性签名
    /// </summary>
    public string Bio { get; set; }
    public BloodPressure BloodPressureMetric { get; set; }
    public HeartRate HeartRateMetric { get; set; }
    public BloodOxygen BloodOxygenMetric { get; set; }
    public RespirationRate RespirationRateMetric { get; set; }

    public UserModel()
    {
        Id = Guid.NewGuid();
        Username = "New User";
        AvatarUrl = "Resources/Images/default_avatar.png";  // 设置默认头像地址
        Bio = "Say something about yourself";
        UserID = "1980103789";
        Gender="女";
        Age="20";
        Weight="51";
        Height="160";
        BloodPressureMetric = new BloodPressure(82,121);
        HeartRateMetric = new HeartRate(78);
        BloodOxygenMetric = new BloodOxygen(98);
        RespirationRateMetric = new RespirationRate(20);

    }
}
