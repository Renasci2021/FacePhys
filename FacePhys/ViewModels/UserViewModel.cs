using System.Windows.Input;
using Android.AccessibilityServices;
using FacePhys.Models;
using FacePhys.Services;

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
    public ICommand RegisterCommand { get; }

    public UserViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoginCommand = new Command(LoginAsync);
        SaveCommand = new Command(async () => 
        {
            
        });
        //RegisterCommand = new Command(RegisterAsync);
    }

    public async Task<T> GetLatestHealthMetricAsync<T>() where T : HealthMetric, new()
    {
        return await _databaseService.GetLatestHealthMetricAsync<T>(_currentUser!.Id);
    }

    public async Task<List<T>> GetHealthMetricsAsync<T>() where T : HealthMetric, new()
    {
        return await _databaseService.GetHealthMetricsAsync<T>(_currentUser!.Id);
    }

    private async void LoginAsync()
    {
        var userId = await _databaseService.GetTestUserAsync();
        User = await _databaseService.GetUserByIdAsync(userId);
        await Shell.Current.GoToAsync("//HomePage");
    }

    public async Task<bool> RegisterAsync(string username, Gender gender, int age, int height, int weight)
    {
        // 验证用户输入，这里简单演示，实际中可能需要更复杂的验证逻
        if (string.IsNullOrWhiteSpace(username))return false; // 输入信息不完整，注册失败

         if (age <= 0 || height <= 0 || weight <= 0)
        {
            return false; // 年龄、身高、体重必须大于零，否则注册失败
        }

        // 检查用户名是否已存在
        var existingUser = await _databaseService.GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            return false; // 用户名已存在，注册失败
        }

        // 创建新用户
        User newUser = new User
        {
            Username = username,
            Age = age, 
            Gender = gender,
            Height = height,
            Weight = weight,
        };

        // 将新用户保存到数据库
        await _databaseService.InsertAsync(newUser);

        return true; // 注册成功
    }

    public async Task AddHealthMetric(HealthMetric metric)
    {
        await _databaseService.SaveHealthMetricAsync(metric);
    }
}
