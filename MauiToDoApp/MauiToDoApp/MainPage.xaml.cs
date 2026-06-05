using MauiToDoApp.Pages;
using MauiToDoApp.Services;

namespace MauiToDoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(TaskService taskService)
        {
            InitializeComponent();
            TasksCollection.ItemsSource = taskService.Tasks;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            // Wir navigieren zur AddTaskPage
            await Shell.Current.GoToAsync(nameof(AddTaskPage));
        }
    }
}
