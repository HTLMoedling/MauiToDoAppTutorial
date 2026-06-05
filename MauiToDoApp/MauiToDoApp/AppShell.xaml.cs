using MauiToDoApp.Pages;

namespace MauiToDoApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
            Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
        }
    }
}
