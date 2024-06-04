using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FacePhys.Models;
// using FacePhys.Data.Repositories.Interfaces;
namespace FacePhys.ViewModels;
public class HealthMetrics{ 
        public BloodPressure BloodPressure { get; set; }
        public BloodOxygen BloodOxygen { get; set; }
        public HeartRate HeartRate { get; set; }
        public RespirationRate RespirationRate { get; set; }
        public DateTime Timestamp { get; set; }
        public HealthMetrics(int diastolic, int systolic, float saturation, int HR_rate, int RR_rate)
        {
            // 初始化时间戳为当前时间
            Timestamp = DateTime.Now;

            BloodPressure = new BloodPressure(){Timestamp = DateTime.Now, Systolic = systolic, Diastolic = diastolic};
            BloodOxygen = new BloodOxygen(){Timestamp = DateTime.Now, Saturation = saturation};
            HeartRate = new HeartRate(){Timestamp = DateTime.Now, Value = HR_rate};
            RespirationRate = new RespirationRate(){Timestamp = DateTime.Now, Value = RR_rate};
        }
    }
public class HomePageViewModel : INotifyPropertyChanged
{
    // 事件

    private HealthMetrics _healthMetrics;

    public HealthMetrics HealthMetrics
    {
        get { return _healthMetrics; }
        set
        {
            if (_healthMetrics != value)
            {
                _healthMetrics = value;
                OnPropertyChanged(nameof(HealthMetrics));
            }
        }
    }
    
    //private readonly IUserRepository _userRepository;

    public UserModel User { get; private set; }

    public ICommand CheckMoreCommand { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public HomePageViewModel(/*IUserRepository userRepository*/)
    {
        // _userRepository = userRepository;

        // User = _userRepository.GetTestUser();

        //_healthMetrics = new HealthMetrics(User.BloodPressureMetric.Diastolic, User.BloodPressureMetric.Systolic, User.BloodOxygenMetric.Saturation, User.HeartRateMetric.Value, User.RespirationRateMetric.Value);
        _healthMetrics = new HealthMetrics(80, 120, 98, 70, 80);

        //_userRepository = userRepository;

        //User = _userRepository.GetTestUser();

        CheckMoreCommand = new Command(async () =>
        {
            if (App.Current?.MainPage != null)
            {
                await App.Current.MainPage.DisplayAlert("查看更多", "查看更多", "确定");
            }
        });

    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
