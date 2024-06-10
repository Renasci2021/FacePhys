using System.Windows.Input;
using FacePhys.Models;
using FacePhys.Services;
using Java.Util;

namespace FacePhys.ViewModels;


public class UserViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private User? _currentUser;

    public User? User
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }
    }


    public bool IsUserLoggedIn => _currentUser != null;

    public ICommand LoginCommand { get; }

    public ICommand SaveCommand { get; }
    public ICommand NavigateToEditInfoCommand { get; } 
    public UserViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoginCommand = new Command(LoginAsync);
        SaveCommand = new Command(async () =>
		{
			// 保存信息
			await SaveUserAsync();
			await Shell.Current.GoToAsync("..");//返回前一页面
		});
        NavigateToEditInfoCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("EditPage");
        });

    }

    private async void LoginAsync()
    {
        var userId = await _databaseService.GetTestUserAsync();
        User = await _databaseService.GetUserByIdAsync(userId);
        await Shell.Current.GoToAsync("//HomePage");
    }

    public async Task<List<HealthMetric?>> LoadHealthMetrics()
    {
        var userId = await _databaseService.GetTestUserAsync();

        var healthMetrics = await _databaseService.GetHealthMetricsByUserIdAsync(userId);
        
        HealthMetric? heartRateMetric = null;
        HealthMetric? bloodPressureMetric = null;
        HealthMetric? bloodOxygenMetric = null;
        HealthMetric? respiratoryRateMetric = null;

        for (int i = 0; i < healthMetrics.Count ; i++)
        {
            var healthMetric = healthMetrics[i];
            switch (healthMetric.Type)
            {
                case MetricType.HeartRate:
                    heartRateMetric = healthMetric;
                    break;
                case MetricType.BloodPressure:
                    bloodPressureMetric = healthMetric;
                    break;
                case MetricType.BloodOxygen:
                    bloodOxygenMetric = healthMetric;
                    break;
                case MetricType.RespiratoryRate:
                    respiratoryRateMetric = healthMetric;
                    break;
            }
        }
        
        // 确保返回顺序
        return new List<HealthMetric?> { heartRateMetric, bloodPressureMetric, bloodOxygenMetric, respiratoryRateMetric };
        //return healthMetrics;
    } 

    public async Task SaveUserAsync()
    {
        await _databaseService.SaveUserAsync(User);
    }
}
