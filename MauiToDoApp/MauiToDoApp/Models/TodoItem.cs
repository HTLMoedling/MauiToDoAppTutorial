using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiToDoApp.Models
{
    public class TodoItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        // Abgabedatum für ToDoItem
        public DateTime DueDate { get; set; } = DateTime.Now;

    }
}
