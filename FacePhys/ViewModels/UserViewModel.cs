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
        var userId = await _databaseService.GetTestUserAsync();
        User = await _databaseService.GetUserByIdAsync(userId);
        await Shell.Current.GoToAsync("//HomePage");
    }
}
