using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SkiaSharp.Views.Maui.Controls.Hosting;
using FacePhys.ViewModels;
using FacePhys.Pages;


namespace FacePhys
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            // builder.Services.AddDbContext<DatabaseContext>(options =>
            // {
            //     options.UseSqlite("Data Source=facephys.db");
            // });
            // builder.Services.AddScoped<DatabaseManager>();
            // builder.Services.AddTransient<DatabaseInitializer>();
            // builder.Services.AddScoped<IUserRepository, UserRepository>();

            // 依赖注入	
            builder.Services.AddScoped<HomePageViewModel>();
            builder.Services.AddScoped<HomePage>();
            builder.Services.AddScoped<MeasurePageViewModel>();
            builder.Services.AddScoped<MeasurePage>();
            builder.Services.AddScoped<ReportPageViewModel>();
            builder.Services.AddScoped<ReportPage>();
            builder.Services.AddScoped<ProfilePageViewModel>();
            builder.Services.AddScoped<ProfilePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();
		    return app;
        }
    }
}
