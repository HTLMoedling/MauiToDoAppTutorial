using MauiToDoApp.Models;
using MauiToDoApp.Pages;
using MauiToDoApp.Services;

namespace MauiToDoApp
{
    public partial class MainPage : ContentPage
    {
        // Die Liste, die sich automatisch in der UI aktualisiert
        private readonly DatabaseService _dbService;
        private readonly TaskService _taskService;

        public MainPage(TaskService taskService, DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            _taskService = taskService;
            TasksCollection.ItemsSource = _taskService.Tasks;
        }

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await _dbService.GetTasksAsync();

            _taskService.Tasks.Clear();
            foreach (var item in items)
                _taskService.Tasks.Add(item);
        }

        private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // 1. Den Sender (die Checkbox) und das dazugehörige TodoItem ermitteln
            if (sender is CheckBox checkBox && checkBox.CommandParameter is TodoItem updatedItem)
            {
                // 2. Den neuen Zustand (True/False) in das Objekt übernehmen
                // e.Value enthält den neuen Zustand der Checkbox
                if (updatedItem.IsDone != e.Value)
                {
                    updatedItem.IsDone = e.Value;

                    // 3. Den geänderten Zustand asynchron in der SQLite-DB speichern (SQL UPDATE)
                    await _dbService.SaveTaskAsync(updatedItem);
                }
            }
        }
    }
}
