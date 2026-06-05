using MauiToDoApp.Models;
using MauiToDoApp.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

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
            if (string.IsNullOrWhiteSpace(TitleEntry.Text)) return;

            var newTask = new TodoItem
            {
                Title = TitleEntry.Text,
                Description = DescEditor.Text,
                IsDone = false,
                DueDate = (DateTime)DueDatePicker.Date // Datum aus der UI übernehmen
            };

            // 1. In der DB speichern
            await _databaseService.SaveTaskAsync(newTask);

            // 2. LOKALE BENACHRICHTIGUNG PLANEN
            // Wir planen die Benachrichtigung z.B. für den Morgen des Abgabetages (oder sofort zum Testen)
            //var notificationTime = newTask.DueDate.Date.AddHours(8); // 08:00 Uhr am Abgabetag

            // Zu Testzwecken kann hier die Zeit auf +10 Sekunden gesetzt werden
            var notificationTime = DateTime.Now.AddSeconds(10);

            var request = new NotificationRequest
            {
                NotificationId = newTask.Id, // Eindeutige ID (nutzt die DB-ID)
                Title = "HTL Deadline Erinnerung! 📝",
                Description = $"Die Abgabe für '{newTask.Title}' steht bevor!",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notificationTime // Wann soll es klingeln?
                }
            };

            // An das Betriebssystem übergeben
            await LocalNotificationCenter.Current.Show(request);

            // Felder leeren und zurück zur MainPage
            TitleEntry.Text = string.Empty;
            DescEditor.Text = string.Empty;
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Eingaben verwerfen und zurückgehen
            TitleEntry.Text = string.Empty;
            DescEditor.Text = string.Empty;

            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}