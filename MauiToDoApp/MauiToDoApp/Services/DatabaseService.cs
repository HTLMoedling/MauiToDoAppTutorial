using MauiToDoApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiToDoApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database is not null)
                return;

            // Der Pfad ist plattformspezifisch (funktioniert auf Android, iOS, Windows)
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TodoTasks.db3");

            _database = new SQLiteAsyncConnection(dbPath);

            // Erstellt die Tabelle nur, wenn sie nicht existiert
            await _database.CreateTableAsync<TodoItem>();
        }

        public async Task<List<TodoItem>> GetTasksAsync()
        {
            await Init();
            return await _database.Table<TodoItem>().ToListAsync();
        }

        public async Task<int> SaveTaskAsync(TodoItem item)
        {
            await Init();
            if (item.Id != 0)
                return await _database.UpdateAsync(item);
            else
                return await _database.InsertAsync(item);
        }

        public async Task<int> DeleteTaskAsync(TodoItem item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }
    }
}
