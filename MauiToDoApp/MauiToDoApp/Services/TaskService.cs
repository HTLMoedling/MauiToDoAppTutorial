using MauiToDoApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiToDoApp.Services
{
    public class TaskService
    {
        // Die zentrale Liste für die gesamte App
        public ObservableCollection<TodoItem> Tasks { get; } = new();
    }
}
