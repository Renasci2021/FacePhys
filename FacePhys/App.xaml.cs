using FacePhys.Services;
using FacePhys.ViewModels;

namespace FacePhys;

public partial class App : Application
{
    static DatabaseService? _databaseService;
    static UserViewModel? _userViewModel;
    static HealthMetricsViewModel? _healthMetricsViewModel;

    public static DatabaseService DatabaseService
    {
        get
        {
            if (_databaseService == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FacePhys.db3");
                _databaseService = new(dbPath);
            }
            return _databaseService;
        }
    }

    public static UserViewModel UserViewModel
    {
        get
        {
            if (_userViewModel == null)
            {
                _userViewModel = new(DatabaseService);
            }
            return _userViewModel;
        }
    }

    public static HealthMetricsViewModel HealthMetricsViewModel
    {
        get
        {
            if(_healthMetricsViewModel ==null )
            {
                _healthMetricsViewModel =  new(DatabaseService);
            }
            return _healthMetricsViewModel;
        }
    }
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

        if (!IsUserLoggedIn())
        {
            Shell.Current.GoToAsync("//LoginPage");
        }
    }

    private bool IsUserLoggedIn()
    {
        return UserViewModel.IsUserLoggedIn;
    }
}
