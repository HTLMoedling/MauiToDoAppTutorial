using MauiToDoApp.Pages;
using MauiToDoApp.Services;
using Microsoft.Extensions.Logging;

namespace MauiToDoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services registrieren
            builder.Services.AddSingleton<TaskService>();

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
