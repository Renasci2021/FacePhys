using FacePhys.Services;

namespace FacePhys;

public partial class App : Application
{
    static DatabaseService? _databaseService;

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
        return false;
    }
}
