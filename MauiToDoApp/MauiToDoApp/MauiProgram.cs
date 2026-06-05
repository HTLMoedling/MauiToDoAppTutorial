using MauiToDoApp.Pages;
using MauiToDoApp.Services;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace MauiToDoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services registrieren
            builder.Services.AddSingleton<TaskService>();
            builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Pages (Wichtig für die TabBar!)
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AddTaskPage>();

            return builder.Build();
        }
    }
}
