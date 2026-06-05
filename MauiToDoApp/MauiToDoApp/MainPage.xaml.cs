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
            // Den Sender (die Checkbox) und das dazugehörige TodoItem ermitteln
            if (sender is CheckBox checkBox && checkBox.CommandParameter is TodoItem updatedItem)
            {
                // Den geänderten Zustand asynchron in der SQLite-DB speichern (SQL UPDATE)
                await _dbService.SaveTaskAsync(updatedItem);
            }
        }

        private async void OnTaskSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // e.CurrentSelection enthält das ausgewählte Element (bzw. mehrere, falls erlaubt)
            if (e.CurrentSelection.FirstOrDefault() is TodoItem clickedTask)
            {
                // 1. Parameter-Dictionary erstellen. Der Schlüssel ("Item") muss exakt 
                // mit dem Namen im [QueryProperty]-Attribut der Detailseite übereinstimmen!
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Item", clickedTask }
                };

                // 2. Navigieren und Parameter übergeben
                await Shell.Current.GoToAsync(nameof(TaskDetailPage), navigationParameter);

                // 3. WICHTIG: Die Auswahl in der UI sofort wieder aufheben, 
                // damit das Event beim nächsten Klick auf dasselbe Item wieder feuert.
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
            }
        }
    }
}
