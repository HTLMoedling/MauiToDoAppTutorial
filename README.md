---
Titel: [MAUI ToDo App Tutorial]
Beschreibung: [Wir erstellen eine ToDo App in MAUI und erweitern die kontinulierlich]
Author: [DI. Niklas Hack]
Datum: [03.06.2026]
---
# Das Grundgerüst & Model
Zuerst definieren wir, wie eine Aufgabe aussieht, und gestalten die Hauptseite.
## Das Model (Models/TodoItem.cs)
Erstelle einen Ordner Models und darin diese Klasse

```csharp
public class TodoItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
}
```

## Die UI der Hauptseite (MainPage.xaml)
Wir nutzen eine CollectionView, um die Aufgaben anzuzeigen, und einen Button, um zur "Hinzufügen"-Seite zu navigieren.
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
x:Class="TodoApp.MainPage"
Title="HTL Aufgaben">
    <Grid RowDefinitions="*, Auto" Padding="20">
        <!-- Liste der Aufgaben -->
        <CollectionView x:Name="TasksCollection" Grid.Row="0">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <HorizontalStackLayout Spacing="15" Padding="10">
                        <CheckBox IsChecked="{Binding IsDone}" />
                        <Label Text="{Binding Title}" VerticalOptions="Center" FontSize="18" />
                    </HorizontalStackLayout>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
        <!-- Button zur Detailseite -->
        <Button Grid.Row="1" Text="Neue Aufgabe" Clicked="OnAddClicked" 
                BackgroundColor="#512BD4" TextColor="White" Margin="0,10,0,0" />
    </Grid>
</ContentPage>

```

# Navigation & Detailseite
Jetzt erstellen wir die Seite, auf der wir neue Aufgaben eingeben.

## Detailseite erstellen (AddTaskPage.xaml)
Rechtsklick aufs Projekt -> Hinzufügen -> Neues Element -> .NET MAUI ContentPage (XAML).
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
x:Class="TodoApp.AddTaskPage"
Title="Aufgabe hinzufügen">
    <VerticalStackLayout Padding="20" Spacing="15">
        <Entry x:Name="TitleEntry" Placeholder="Titel der Aufgabe" />
        <Editor x:Name="DescEditor" Placeholder="Beschreibung" HeightRequest="100" />        
        <Button Text="Speichern" Clicked="OnSaveClicked" />
        <Button Text="Abbrechen" Clicked="OnCancelClicked" BackgroundColor="Gray" />
    </VerticalStackLayout>
</ContentPage>
```

## Routen registrieren (AppShell.xaml.cs)
Damit MAUI weiß, wo die AddTaskPage zu finden ist

```csharp
public AppShell()
{
    InitializeComponent();
    Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
}
```

## Der Daten-Service (Services/TaskService.cs)
Erstelle einen Ordner Services. Dieser Service hält die Daten zentral bereit.

```csharp
using System.Collections.ObjectModel;
using TodoApp.Models;

namespace TodoApp.Services;

public class TaskService
{
    // Die zentrale Liste für die gesamte App
    public ObservableCollection<TodoItem> Tasks { get; } = new();
}
```

## Registrierung in der MauiProgram.cs
Hier sagen wir der App, dass der Service und die Seiten existieren und nur einmal (als Singleton) erstellt werden sollen.

```csharp
// Services registrieren
builder.Services.AddSingleton<TaskService>();
```

## Logik der MainPage (MainPage.xaml.cs)

```csharp
using System.Collections.ObjectModel;
using TodoApp.Models;

namespace TodoApp;

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
```

## Logik der AddTaskPage (AddTaskPage.xaml.cs)
Hier nutzen wir einen kleinen "Trick": Wir greifen auf die Liste der TaskService zu, um die Daten zu speichern.

```csharp
using TodoApp.Models;

namespace TodoApp;

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
```

Zusammenfassung für die Schüler:
1.	Model: Beschreibt die Datenstruktur.
2.	XAML: Definiert das Aussehen (Grid, StackLayout, Controls).
3.	Shell Navigation: Ermöglicht das Springen zwischen Seiten mittels GoToAsync.
4.	ObservableCollection: Sorgt dafür, dass die Liste in der UI sofort erscheint, wenn sie im Code hinzugefügt wird.


# Tabbar für die ToDo App

Anstatt nur einen einfachen ShellContent zu haben, gruppieren wir die Seiten in einer TabBar.

## Tabbar in AppShell.xaml hinzufügen
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="MauiTodoApp.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:MauiTodoApp"
    xmlns:pages="clr-namespace:MauiTodoApp.Pages"
    Title="MauiTodoApp"
    Shell.FlyoutBehavior="Disabled">

    <TabBar>
        <!-- Tab 1: Die Aufgabenliste -->
        <Tab Title="Aufgaben" Icon="list_icon.png">
            <ShellContent 
                ContentTemplate="{DataTemplate local:MainPage}" 
                Route="MainPage" />
        </Tab>

        <!-- Tab 2: Neue Aufgabe hinzufügen -->
        <Tab Title="Hinzufügen" Icon="add_icon.png">
            <ShellContent 
                ContentTemplate="{DataTemplate pages:AddTaskPage}" 
                Route="AddTaskPage" />
        </Tab>
    </TabBar>

</Shell>
```

*Hinweis: Wenn du noch keine Icons hast, kannst du die Zeile Icon="..." weglassen oder Standard-Bilder aus dem Resources/Images Ordner nehmen.*

## Anpassung in der MauiProgram.cs
Damit die Shell die Seiten korrekt über Dependency Injection auflösen kann, müssen beide Seiten als Singleton oder Transient registriert sein (haben wir im vorherigen Schritt schon gemacht, hier zur Sicherheit nochmal die Übersicht):

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder.UseMauiApp<App>()
           .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
           });

    // Zentraler Daten-Service
    builder.Services.AddSingleton<TaskService>();

    // Pages (Wichtig für die TabBar!)
    builder.Services.AddSingleton<MainPage>();
    builder.Services.AddSingleton<AddTaskPage>(); 

    return builder.Build();
}
```

# SQLite einbinden
Der nächste logische Schritt für eine "echte" App ist es die Daten in einer Datenbank zu speichern, damit diese nach dem Schließen der App nicht verworfen werden. Um SQLite in .NET MAUI zu nutzen, verwenden wir das NuGet-Paket sqlite-net-pcl.

Zusätzlich werden wir das Daten-Service so erweitert, dass die Datenbank beim ersten Start automatisch angelegt wird.

## Vorbereitung (NuGet)
Installiere das Paket in deinem Projekt: install-package sqlite-net-pcl
## Das Model mit Datenbank-Attributen
Damit SQLite weiß, was der Primärschlüssel ist, fügen wir Attribute hinzu:

```csharp
using SQLite;

namespace TodoApp.Models;

public class TodoItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
}
```

## Der Database Service (Services/DatabaseService.cs)
Dieser Service prüft bei jedem Aufruf (Init), ob die Datenbankverbindung bereits steht. Wenn nicht, baut er sie auf und erstellt die Tabelle TodoItem (falls sie noch nicht existiert).

```csharp
using SQLite;
using TodoApp.Models;

namespace TodoApp.Services;

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
```

Den Service als Singleton registrieren (wird einmal für die App erstellt) 

```csharp
builder.Services.AddSingleton<DatabaseService>();
```

## Integration in die UI (MainPage.xaml.cs)
In der MainPage.xaml.cs laden wir die Daten nun aus der Datenbank statt aus einer statischen Liste.

Füge die Services hinzu und injecte diese im Konstruktor

```csharp
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
```

Füge die Methode OnAppearing hinzu.
Diese Methode wird in dem Moment aufgerufen, in dem eine Seite auf dem Bildschirm erscheint oder für den Benutzer sichtbar wird.

```csharp
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var items = await _dbService.GetTasksAsync();

        _taskService.Tasks.Clear();
        foreach (var item in items)
            _taskService.Tasks.Add(item);
    }
```

## Mainpage.xaml.cs nach dem Update

```csharp
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
```

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

```csharp
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var items = await _dbService.GetTasksAsync();

        _taskService.Tasks.Clear();
        foreach (var item in items)
            _taskService.Tasks.Add(item);
    }
```

## Update AddTaskPage.xaml.cs
Füge das DatabadeServices hinzu und injecte dieses im Konstruktor

```csharp
    private readonly TaskService _taskService;
    private readonly DatabaseService _databaseService;

    public AddTaskPage(TaskService taskService, DatabaseService databaseService)
    {
        InitializeComponent();
        _taskService = taskService;
        _databaseService = databaseService;
    }
```

Update der OnSaveClicked Methode, damit die Daten jetzt in der Datenbank gespeichert werden

```csharp
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
```

Update der ONCancelClick Methode damit bei einem Klick auf Cancel die Eingabe gelöscht wird

```csharp
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Eingaben verwerfen und zurückgehen
        TitleEntry.Text = string.Empty;
        DescEditor.Text = string.Empty;

        await Shell.Current.GoToAsync("..");
    }
```

## AddTaskPage.xaml.cs nach dem Update

```csharp
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
```

