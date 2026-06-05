using MauiToDoApp.Models;
using MauiToDoApp.Services;

namespace MauiToDoApp.Pages
{
    public partial class AddTaskPage : ContentPage
    {
        private readonly TaskService _taskService;
        private readonly DatabaseService _databaseService;

        public AddTaskPage(TaskService taskService, DatabaseService databaseService)
        {
            InitializeComponent();
            _taskService = taskService;
            _databaseService = databaseService;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validierung: Wenn kein Titel eingegeben wurde, abbrechen
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlertAsync("Fehler", "Bitte gib einen Titel ein.", "OK");
                return;
            }

            // Neues Datenobjekt erstellen
            var newTask = new TodoItem
            {
                Title = TitleEntry.Text,
                Description = DescEditor.Text,
                IsDone = false // Neue Aufgaben sind standardmäßig offen
            };

            // In der SQLite-Datenbank speichern (asynchron!)
            await _databaseService.SaveTaskAsync(newTask);

            // Eingabefelder für die nächste Nutzung leeren
            TitleEntry.Text = string.Empty;
            DescEditor.Text = string.Empty;

            // Zurück zur vorherigen Seite (MainPage), wo sich die Liste im "OnAppearing" neu lädt
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Eingaben verwerfen und zurückgehen
            TitleEntry.Text = string.Empty;
            DescEditor.Text = string.Empty;

            await Shell.Current.GoToAsync("..");
        }
    }
}