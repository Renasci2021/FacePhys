using System.Windows.Input;
using FacePhys.Models;
using FacePhys.Services;

namespace FacePhys.ViewModels;

public class UserViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private User? _currentUser;

    public User? CurentUser
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
        if (userList.Count == 0)
        {
            var user = new User
            {
                Name = "Test",
            };
            await _databaseService.SaveUserAsync(user);
            CurentUser = user;
        }
        else
        {
            CurentUser = userList[0];
        }
        await Shell.Current.GoToAsync("//HomePage");
    }
}
