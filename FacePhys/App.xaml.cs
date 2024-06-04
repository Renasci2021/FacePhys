namespace FacePhys
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}

// using FacePhys.Data;

// namespace FacePhys;

// public partial class App : Application
// {
// 	public App(DatabaseInitializer dbInitializer)
// 	{
// 		InitializeComponent();

// 		MainPage = new AppShell();

// 		dbInitializer.Initialize();
// 	}
// }

