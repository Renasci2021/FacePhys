using System.Windows.Input;
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

    public UserViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoginCommand = new Command(LoginAsync);
    }

    private async void LoginAsync()
    {
        // 创建测试用户
        var userList = await _databaseService.GetUsersAsync();
        foreach (var user in userList)
        {
            await _databaseService.DeleteUserAsync(user);
        }
        var testUser = new User
        {
            Username = "Test User",
            Gender = Gender.Female,
        };
        await _databaseService.SaveUserAsync(testUser);
        User = testUser;
        await Shell.Current.GoToAsync("//HomePage");
    }
}
