using MauiToDoApp.Models;
using MauiToDoApp.Services;

namespace MauiToDoApp.Pages;

public partial class AddTaskPage : ContentPage
{
    private readonly TaskService _taskService;

    public AddTaskPage(TaskService taskService)
    {
        InitializeComponent();
        _taskService = taskService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            _taskService.Tasks.Add(new TodoItem
            {
                Title = TitleEntry.Text,
                Description = DescEditor.Text
            });
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}