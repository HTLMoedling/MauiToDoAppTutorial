using MauiToDoApp.Models;

namespace MauiToDoApp.Pages;

// Das Attribut verknüpft den Query-Namen "Item" mit der C#-Property "SelectedTask"
[QueryProperty(nameof(SelectedTask), "Item")]
public partial class TaskDetailPage : ContentPage
{
    private TodoItem _selectedTask;

    public TodoItem SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            // Sobald die Daten reinkommen, befüllen wir die UI-Elemente
            UpdateUI();
        }
    }

    public TaskDetailPage()
    {
        InitializeComponent();
    }

    private void UpdateUI()
    {
        if (SelectedTask is null) return;

        TitleLabel.Text = SelectedTask.Title;
        DescLabel.Text = string.IsNullOrWhiteSpace(SelectedTask.Description)
            ? "Keine Beschreibung vorhanden."
            : SelectedTask.Description;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        // Da diese Seite per "GoToAsync" oben auf den Stack gelegt wird,
        // bringt uns ".." sauber wieder zurück!
        await Shell.Current.GoToAsync("..");
    }
}
