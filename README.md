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
Erstelle einen neuen Ordner Pages
Rechtsklick aufs den Ordner -> Hinzufügen -> Neues Element -> .NET MAUI ContentPage (XAML).
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
x:Class="TodoApp.Pages.AddTaskPage"
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

namespace TodoApp.Pages;

public partial class AddTaskPage : ContentPage
{
    private TaskService _taskService;

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
```

## Update AddTaskPage.xaml.cs
Füge das DatabaseServices hinzu und injecte dieses im Konstruktor

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

Update der OnCancelClick Methode damit bei einem Klick auf Cancel die Eingabe gelöscht wird

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

# Checkbox-Zustand in DB speichern
Damit der Zustand der Checkbox in der Datenbank gespeichert wird, müssen wir der Checkbox sagen, dass sie bei einer Statusänderung eine Methode im Code-Behind aufrufen soll. Dazu nutzen wir das Event CheckedChanged.
Zusätzlich übergeben wir auch hier das aktuelle TodoItem als CommandParameter, damit wir im Code-Behind wissen, zu welcher Aufgabe die geklickte Checkbox gehört.

## Die UI anpassen (MainPage.xaml)
Suche in der MainPage.xaml nach der <CheckBox ... /> und passe sie wie folgt an:

```xml
<CheckBox IsChecked="{Binding IsDone}"
          CheckedChanged="OnTaskCheckedChanged"
          CommandParameter="{Binding .}" />
```

## Die Logik implementieren (MainPage.xaml.cs)
Im Code-Behind fangen wir das Event ab. Da wir im DatabaseService bereits die Methode SaveTaskAsync(item) so intelligent geschrieben haben, dass sie automatisch ein Update in der SQL-Datenbank ausführt, wenn das Item bereits eine Id besitzt, müssen wir dem Service einfach nur das aktualisierte Objekt übergeben.
Füge diese Methode in die MainPage.xaml.cs ein:

```csharp
private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
{
    // Den Sender (die Checkbox) und das dazugehörige TodoItem ermitteln
    if (sender is CheckBox checkBox && checkBox.CommandParameter is TodoItem updatedItem)
    {
        // Den geänderten Zustand asynchron in der SQLite-DB speichern (SQL UPDATE)
        await _dbService.SaveTaskAsync(updatedItem);
    }
}
```

# Leere Listen abfangen (EmptyView):
Das Abfangen von leeren Zuständen (sogenannte Zero-Data States oder Empty States) ist ein entscheidender Schritt für eine professionelle User Experience (UX). Eine App muss auch dann gut aussehen, wenn noch keine Daten vorhanden sind.
In .NET MAUI ist das dank der EmptyView-Property der CollectionView extrem einfach gelöst – man benötigt dafür nicht einmal eine einzige Zeile Code-Behind.

## Die UI erweitern (MainPage.xaml)
Die CollectionView besitzt das Attribut EmptyView. Dort kann man entweder einen einfachen Text (String) hineinschreiben oder über CollectionView.EmptyView ein komplett eigenes XAML-Layout (z. B. mit einem Icon und einer genaueren Beschreibung) definieren.

Suche in der MainPage.xaml nach deiner <CollectionView ...> und passe sie wie folgt an:

```xml
<CollectionView x:Name="TasksCollection" Grid.Row="0">
    
    <CollectionView.EmptyView>
        <VerticalStackLayout VerticalOptions="Center" 
                             HorizontalOptions="Center" 
                             Spacing="10" 
                             Padding="30">
            
            <Label Text="Keine Aufgaben vorhanden!" 
                   FontSize="20" 
                   FontAttributes="Bold" 
                   HorizontalTextAlignment="Center" 
                   TextColor="{AppThemeBinding Light=#512BD4, Dark=#A294F9}" />
            
            <Label Text="Erstelle deine erste Aufgabe über den Tab 'Hinzufügen' oder den Button unten." 
                   FontSize="14" 
                   HorizontalTextAlignment="Center" 
                   TextColor="Gray" />
        </VerticalStackLayout>
    </CollectionView.EmptyView>

    <CollectionView.ItemTemplate>
        <DataTemplate>
            ...
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

# Datenübergabe an eine Detailseite (Query Property Attribut):
Durch parametrisierte Navigation ist es möglich Daten sicher zwischen verschiedenen Bildschirmen zu transportiert, ohne auf unsaubere globale Variablen zurückgreifen zu müssen.
In .NET MAUI funktioniert das über Query-Parameter, ähnlich wie man es von URLs aus der Webentwicklung kennt (z. B. seite?parameter=wert).

## Eine neue Detailseite erstellen (TaskDetailPage.xaml)
Füge dem Projekt eine neue .NET MAUI ContentPage (XAML) mit dem Namen TaskDetailPage.xaml hinzu. Diese Seite soll die Details einer ausgewählten Aufgabe anzeigen.

```xml
<VerticalStackLayout Padding="20" Spacing="20">
    <Label x:Name="TitleLabel" FontSize="24" FontAttributes="Bold" TextColor="#512BD4" />

    <Label x:Name="StatusLabel" FontSize="14" FontAttributes="Italic" TextColor="Gray" />

    <BoxView HeightRequest="1" BackgroundColor="LightGray" />

    <Label Text="Beschreibung:" FontSize="14" FontAttributes="Bold" />
    <Label x:Name="DescLabel" FontSize="16" />

    <Button Text="Schließen" Clicked="OnCloseClicked" Margin="0,20,0,0" />
</VerticalStackLayout>
```

## Die Logik mit QueryProperty (TaskDetailPage.xaml.cs)
Hier kommt die Magie von MAUI ins Spiel. Wir nutzen das [QueryProperty]-Attribut über der Klasse. Es fängt den Parameter aus der Navigation ab und schleust ihn automatisch in eine Property (hier SelectedTask) ein.

```csharp
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
```

## Route registrieren (AppShell.xaml.cs)
Da die Detailseite kein fester Tab ist, sondern dynamisch aufgerufen wird, müssen wir ihre Route im Konstruktor der AppShell.xaml.cs registrieren.

```csharp
Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
```

## Klick-Event auf der Hauptseite einbauen (MainPage.xaml)
Wir müssen der CollectionView mitteilen, dass etwas passieren soll, wenn ein Schüler auf einen Eintrag tippt. Dazu nutzen wir das Attribut SelectionChanged und stellen den SelectionMode auf Single.
Suche die <CollectionView> in der MainPage.xaml und passe sie an.

```xml
<CollectionView x:Name="TasksCollection" 
                Grid.Row="0"
                SelectionMode="Single"
                SelectionChanged="OnTaskSelectionChanged">
```

## Die Navigation auslösen (MainPage.xaml.cs)
Im Code-Behind der MainPage fangen wir das Auswahl-Event ab, extrahieren das angeklickte TodoItem und übergeben es in einem Dictionary als Navigationsparameter.
Füge diese Methode in die MainPage.xaml.cs ein.

```csharp
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
```

# Push Benachrichtigungen für die ToDos
Durch Push-Notifications werden Nutzer durch eine Nachricht direkt am Gerät an ihre Aufgaben erinnern. Da MAUI keine eigene "Push-Engine" mitbringt, nutzt man dafür in der Regel lokale Benachrichtigungen (wenn die App auf dem Gerät läuft) oder Remote Push Notifications (über einen Server wie Firebase).
Benachrichtigungen werden auch dann geschickt, wenn die App nicht geöffnet ist
Wir nutzen dafür das bewährte und aktuelle NuGet-Paket Plugin.LocalNotification.

## NuGet-Paket installieren
Zuerst müssen wir das Paket zum Projekt hinzufügen: 

```xml
install-package Plugin.LocalNotification
```

## Android Manifest anpassen
Manchmal verweigert Android den Dialog, wenn die Berechtigung nicht auch im "Manifest" angekündigt wurde.

1.	Navigiere im Projektmappen-Explorer zu: Platforms -> Android -> AndroidManifest.xml.
2.	Klicke mit der rechten Maustaste darauf und wähle "Code anzeigen".
3.	Füge innerhalb des <manifest>-Tags (aber außerhalb des <application>-Tags) folgende Zeile hinzu.

Die Datei kann auch direkt im Explorer geöffnet (Notepad++) und bearbeitet werden

```xml
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

## Paket in MauiProgram.cs initialisiert 
Registriere die Notification im builder (.UseLocalNotification())
```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseLocalNotification()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    });
```

## Das Model erweitern (Models/TodoItem.cs)
Wir fügen ein Feld für das Fälligkeitsdatum hinzu.

```csharp
    // Abgabedatum für ToDoItem
    public DateTime DueDate { get; set; } = DateTime.Now;
```

## UI anpassen (AddTaskPage.xaml)
Wir fügen einen DatePicker hinzu, damit wir das Abgabedatum komfortabel über den nativen Kalender des Smartphones auswählen können.

```xml
<Label Text="Abgabedatum / Deadline:" FontAttributes="Bold" />
    <DatePicker x:Name="DueDatePicker" Format="dd.MM.yyyy" MinimumDate="{x:Static sys:DateTime.Now}" />
```

Damit sys erkannt wird, muss im Header der Namespace hinzugefügt werden

```xml
xmlns:sys="clr-namespace:System;assembly=mscorlib"
```

## Die Benachrichtigung planen (AddTaskPage.xaml.cs)
Beim Speichern erstellen wir nun zusätzlich zur Datenbank-Zeile eine Benachrichtigung, die exakt zur Wunschzeit vom Betriebssystem abgefeuert wird – selbst wenn die App geschlossen ist.

```csharp
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
    var notificationTime = newTask.DueDate.Date.AddHours(8); // 08:00 Uhr am Abgabetag

    // Zu Testzwecken kann hier die Zeit auf +10 Sekunden gesetzt werden
    // var notificationTime = DateTime.Now.AddSeconds(10);

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
    await Shell.Current.GoToAsync("//MainPage");
}
```

## Update TaskDetailPage
Damit das Fälligkeitsdatum angezeigt wird, müssen wir die TaskDetailPage anpassen

Update Methode UpdateUI() in TaskDetailPage.xaml.cs
```csharp
StatusLabel.Text = SelectedTask.IsDone 
            ? "Status: Erledigt ✅" 
            : $"Status: Offen (Fällig am: {SelectedTask.DueDate.ToString("dd.MM.yyyy")}) ⏳";
```

## Berechtigung für Push-Notification bei Start der App
Wir wollen dass beim aler esrsten Start der App, der Neutzer gefragt wird, ob er Push-Notifications zulassen will.
Hier für müssen wir die MainPage.xaml.cs Datei anpassen und diese Funktionalität einbauen.
Füge folgende 2 Methoden hinzu:

```csharp
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await AskNotificationPermissionOnceAsync();
    }

    private async Task AskNotificationPermissionOnceAsync()
    {
        if (_notificationPermissionChecked)
            return;

        _notificationPermissionChecked = true;

        bool alreadyAsked = Preferences.Default.Get("AskedNotificationPermission", false);

        if (alreadyAsked)
            return;

        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }

        Preferences.Default.Set("AskedNotificationPermission", true);
    }
```

# Verbessern der Navigation
Wenn man auf der MainPage.xaml auf Aufgabe hinzufügen klickt, wird nicht in der Tab (AddTaskPage.xaml gewechsel (ersichtlich daran, dass der Tab nicht selektiert wird) sonder die neue Seite wird auf die MainPage gelegt (Push-Navigation).

Um dies zu ändern, müssen wir beim Routing mit absoluten Pfaden arbeiten.

## Update MainPage.xaml.cs

```csharp
private async void OnAddClicked(object sender, EventArgs e)
{
    // Wir navigieren zur AddTaskPage
    await Shell.Current.GoToAsync($"//{nameof(AddTaskPage)}");
}
```

## Update AddTaskPage.xaml.cs
Ändere GoToAsync zu:

```csharp
await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
```


# Validierung des User Inputs
Für eine optimale Usability der App sollte jeder Input direkt vaidiert werden und Fehler dem User unmittelbar bei der Eingabe in der UI angezeigt werden.

Um die Validierung komplett vom Code-Behind zu trennen, verwenden wir Behaviors. Ein Behavior ist eine Klasse, die sich an ein UI-Element (wie ein Entry) "anhängt" und dessen Verhalten überwacht – völlig unabhängig von deiner Page-Logik.

## Das EntryValidationBehavior (Die Logik-Klasse)
Erstelle einen neuen Ordner Common und darin eine neue Klasse EntryValidationBehavior. 
Diese prüft den Text, während der Nutzer tippt.

```csharp
public class EntryValidationBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnEntryTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        // Beispiel: Titel muss mindestens 3 Zeichen lang sein
        bool isValid = !string.IsNullOrWhiteSpace(e.NewTextValue) && e.NewTextValue.Length >= 3;

        // Visuelles Feedback über den VisualState
        VisualStateManager.GoToState(entry, isValid ? "Normal" : "Invalid");
    }
}
```

## Die XAML-Einbindung (Keine Logik im Code-Behind)
Jetzt müssen wir in der AddTaskPage.xaml nur noch das Behavior zuweisen. Der Code-Behind bleibt für diesen Teil komplett leer.

```xml
<Entry x:Name="TitleEntry" Placeholder="Titel der Aufgabe">
    <Entry.Behaviors>
        <local:EntryValidationBehavior />
    </Entry.Behaviors>
</Entry>
```

# MAUI App auf MVVM umstellen
Die Umstellung von Code-Behind (Ereignis-basiert) auf das MVVM-Pattern (Model-View-ViewModel) ist ein wichtiger Schritt um den Code testbar, wartbar und sauberer zu machen.

## Grundkonzept: Was ändert sich?
- View (XAML): Enthält nur noch das Layout. Keine x:Name für UI-Elemente mehr, stattdessen DataBinding.
- ViewModel (C#): Enthält die Logik, Properties für die UI und Commands statt Clicked-Events.
- Model: Bleibt gleich (dein TodoItem).

## Vorbereitung (CommunityToolkit.Mvvm)
Installiere das NuGet-Paket CommunityToolkit.Mvvm. 
Es ist der Standard für MAUI und nimmt dir durch Source Generators extrem viel Arbeit ab (z. B. [ObservableProperty] für INotifyPropertyChanged).

```xml
install-package CommunityToolkit.Mvvm
```

## Basis-ViewModel erstellen
Erstelle einen neuen Ordner ViewModels
Erstelle in diesem Ordner eine Klasse BaseViewModel, von der alle deine ViewModels erben.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiToDoApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isBusy;
}
```

### [ObservableProperty]
Das Community Toolkit nutzt C#-Source-Generatoren. Wenn du ein Attribut auf ein privates Feld setzt, generiert der Compiler im Hintergrund automatisch die passende öffentliche Eigenschaft für dich.

## MainPageViewModel
Erstelle im selber Ordner eine Klasse MainPageViewModel.cs.
Verschiebe die Logik aus MainPage.xaml.cs hierher.

Folgende Schritte müssen wir nun durchführen:

Leite die Klasse von BaseViewModel ab

```csharp
public partial class MainPageViewModel : BaseViewModel
```


Füge ein Field für das Databaseservice hinzu
Füge weiters eine neue Property Tasks hinzu. Somit können wir aus der View direkt auf diese Property mit allen tasks binden

```csharp
private readonly DatabaseService _dbService;
public ObservableCollection<TodoItem> Tasks { get; } = new();
```

Erstelle nun den Konstruktor und die Methode LoadTasksAsync()

```csharp
public MainPageViewModel(DatabaseService dbService)
{
    _dbService = dbService;
    Title = "HTL Aufgaben";
    LoadTasksAsync();
}

private async Task LoadTasksAsync()
{
    var items = await _dbService.GetTasksAsync();
    Tasks.Clear();
    foreach (var item in items) Tasks.Add(item);
}
```
Hier setzen wir den Titel für unsere View (ObservableProperty aus BaseViewModel)
Weiters werden in LoadTasksAsync() alle Tasks aus der Datebank geladen.

Erstelle nun die restlichen Methoden um direkt aus der View diese als Commands aufzurufen

```csharp
[RelayCommand]
private async Task ToggleTaskStatus(TodoItem item)
{
    if (item == null) return;
    await _dbService.SaveTaskAsync(item);
}

[RelayCommand]
private async Task NavigateToDetails(TodoItem item)
{
    if (item == null) return;
    await Shell.Current.GoToAsync(nameof(TaskDetailPage), new Dictionary<string, object> { { "Item", item } });
}

[RelayCommand]
private async Task GoToAddPage() => await Shell.Current.GoToAsync($"//{nameof(AddTaskPage)}");
```

In der Welt von MVVM ist das RelayCommand (im Community Toolkit [RelayCommand] oder die Klasse RelayCommand) das Gegenstück zu ObservableProperty. Während ObservableProperty Datenflüsse von deinem ViewModel zur View (UI) steuert, steuert das RelayCommand Benutzerinteraktionen von der View zurück zum ViewModel.

> [!NOTE]
> Um das Command zu deaktivieren/aktivieren kann dies mittels CanExecute realisiert werden.

>```csharp
>[RelayCommand(CanExecute = nameof(CanSave))]
>private void Save() { /* ... */ }
>private bool CanSave() => !string.IsNullOrEmpty(Name);
>```

Nun können wir alle Methoden der MainPage.xaml.cs löschen.
Nun injecten wir das Viewmodel im Konstruktor und weisen es dem BindingContext zu

```csharp
using MauiToDoApp.ViewModels;

namespace MauiToDoApp;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
```

Nun passen wir die MainPage.xaml folgendermaßen an, sodass wir nichtmehr auf das Codebehind File zugreifen, sondern nur noch auf das ViewModel

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:models="clr-namespace:MauiToDoApp.Models"
             xmlns:vm="clr-namespace:MauiToDoApp.ViewModels"
             x:Class="MauiToDoApp.MainPage"
             Title="{Binding Title}">

    <Grid RowDefinitions="*, Auto" Padding="20">
        <!-- Liste der Aufgaben -->
        <CollectionView Grid.Row="0"
                        ItemsSource="{Binding Tasks}"
                        SelectionMode="Single"
                        SelectionChangedCommand="{Binding NavigateToDetailsCommand}"
                        SelectionChangedCommandParameter="{Binding Source={RelativeSource Self}, Path=SelectedItem}">
            <CollectionView.EmptyView>
                <VerticalStackLayout VerticalOptions="Center" 
                 HorizontalOptions="Center" 
                 Spacing="10" 
                 Padding="30">

                    <Label Text="Keine Aufgaben vorhanden!" 
                        FontSize="20" 
                        FontAttributes="Bold" 
                        HorizontalTextAlignment="Center" 
                        TextColor="{AppThemeBinding Light=#512BD4, Dark=#A294F9}" />

                    <Label Text="Erstelle deine erste Aufgabe über den Tab 'Hinzufügen' oder den Button unten." 
                        FontSize="14" 
                        HorizontalTextAlignment="Center" 
                        TextColor="Gray" />
                </VerticalStackLayout>
            </CollectionView.EmptyView>
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:TodoItem">
                    <HorizontalStackLayout Spacing="15" Padding="10">
                        <CheckBox IsChecked="{Binding IsDone}"
                                  Command="{Binding Source={RelativeSource AncestorType={x:Type vm:MainPageViewModel}}, Path=ToggleTaskStatusCommand}"
                                  CommandParameter="{Binding .}" />
                        <Label Text="{Binding Title}" VerticalOptions="Center" FontSize="18" />
                    </HorizontalStackLayout>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <Button Grid.Row="1" Text="Neue Aufgabe" 
                Command="{Binding GoToAddPageCommand}" 
                BackgroundColor="#512BD4" TextColor="White" Margin="0,10,0,0" />
    </Grid>
</ContentPage>
```

Zu guter letzt müssen wir das ViewModel nur  och im File MauiProgram.cs registrieren

```csharp
builder.Services.AddSingleton<MainPageViewModel>();
```
## AddTaskPageViewModel
Nun stellen wir die AddTaskPage auf MVVM um.

Erstelle ein neues File AddTaskPageViewModel.cs im Ordner ViewModels mit folgendem Inhalt

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Models;
using MauiToDoApp.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace MauiToDoApp.ViewModels;

public partial class AddTaskPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty] private string _title;
    [ObservableProperty] private string _description;
    [ObservableProperty] private DateTime _dueDate = DateTime.Now;

    public AddTaskPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;

        var newTask = new TodoItem
        {
            Title = Title,
            Description = Description,
            IsDone = false,
            DueDate = DueDate
        };

        await _databaseService.SaveTaskAsync(newTask);

        // Benachrichtigung planen
        await LocalNotificationCenter.Current.Show(new NotificationRequest
        {
            NotificationId = newTask.Id,
            Title = "HTL Deadline Erinnerung! 📝",
            Description = $"Die Abgabe für '{newTask.Title}' steht bevor!",
            Schedule = new NotificationRequestSchedule { NotifyTime = DateTime.Now.AddSeconds(10) }
        });

        // Felder zurücksetzen
        Title = string.Empty;
        Description = string.Empty;

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
}
```

### AddTaskPage.xaml
Wir passen nun die AddTaskPage.xaml so an, dass wir die Eingabefelder und Buttons nun via DataBinding mit dem ViewModel verbinden.

 ```xml
 <ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:sys="clr-namespace:System;assembly=mscorlib"
             x:Class="MauiToDoApp.Pages.AddTaskPage"
             Title="Aufgabe hinzufügen">
    <VerticalStackLayout Padding="20" Spacing="15">
        <Entry Text="{Binding Title}" Placeholder="Titel der Aufgabe" />
        <Editor Text="{Binding Description}" Placeholder="Beschreibung" HeightRequest="100" />

        <Label Text="Abgabedatum / Deadline:" FontAttributes="Bold" />
        <DatePicker Date="{Binding DueDate}" Format="dd.MM.yyyy" />

        <Button Text="Speichern" Command="{Binding SaveCommand}" />
        <Button Text="Abbrechen" Command="{Binding CancelCommand}" BackgroundColor="Gray" />
    </VerticalStackLayout>
</ContentPage>
```

### AddTaskPage.xaml.cs
Lösche nun wieder alle Methoden im Codebehind File und weise nun dem BindingContex das ViewModel zu.

```csharp
using MauiToDoApp.ViewModels;

namespace MauiToDoApp.Pages
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage(AddTaskPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
```

Registriere nun wieder das ViewModel in MauiProgram.cs

```csharp
builder.Services.AddSingleton<AddTaskPageViewModel>();
```

## TaskDetailViewModel
Jetzt machen wir das selbe mit der TaskDetailPage

### TaskDetailPageViewModel
Das ViewModel erhält das TodoItem via QueryProperty und stellt berechnete Properties für die UI bereit.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Models;

namespace MauiToDoApp.ViewModels;

[QueryProperty(nameof(SelectedTask), "Item")]
public partial class TaskDetailPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private TodoItem _selectedTask;

    // Wir berechnen den Status-Text hier im ViewModel statt in der View
    public string StatusText => SelectedTask?.IsDone == true
        ? "Status: Erledigt ✅"
        : $"Status: Offen (Fällig am: {SelectedTask?.DueDate:dd.MM.yyyy HH:mm}) ⏳";

    [RelayCommand]
    private async Task Close() => await Shell.Current.GoToAsync("..");
}
```

> [!NOTE]
>Das [QueryProperty]-Attribut ist ein zentrales Werkzeug in MVVM-Frameworks (insbesondere in .NET MAUI), um Daten zwischen verschiedenen >Seiten (Pages) einer App zu übergeben.
>Stell es dir wie eine "Paketübergabe" beim Navigieren vor: Wenn du von Seite A zu Seite B navigierst, kannst du ein Objekt oder eine ID >mitgeben, die Seite B dann automatisch entgegennimmt.

> [!NOTE]
> Dein StatusText nutzt SelectedTask direkt. Wenn sich SelectedTask ändert (durch das QueryProperty-Mapping), weiß die View nichts davon, > weil sich deine Eigenschaft StatusText nicht von selbst aktualisiert. Sie wird nur einmal beim Laden der Seite berechnet.
> Die Lösung: Nutze [NotifyPropertyChangedFor]

### TaskDetailPage.xaml

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiToDoApp.Pages.TaskDetailPage"
             Title="Aufgaben-Details">

    <VerticalStackLayout Padding="20" Spacing="20">
        <Label Text="{Binding SelectedTask.Title}" FontSize="24" FontAttributes="Bold" TextColor="#512BD4" />

        <Label Text="{Binding StatusText}" FontSize="14" FontAttributes="Italic" TextColor="Gray" />

        <BoxView HeightRequest="1" BackgroundColor="LightGray" />

        <Label Text="Beschreibung:" FontSize="14" FontAttributes="Bold" />
        <Label Text="{Binding SelectedTask.Description, TargetNullValue='Keine Beschreibung vorhanden.'}" FontSize="16" />

        <Button Text="Schließen" Command="{Binding CloseCommand}" Margin="0,20,0,0" />
    </VerticalStackLayout>
</ContentPage>
```

### TaskDetailPage.xaml.cs

```csharp
using MauiToDoApp.ViewModels;

namespace MauiToDoApp.Pages;

public partial class TaskDetailPage : ContentPage
{
    public TaskDetailPage(TaskDetailPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
```

### MauiProgram.cs

```csharp
builder.Services.AddTransient<TaskDetailPage>();
builder.Services.AddTransient<TaskDetailPageViewModel>();
```

# Edit Task
Wir wollen nun die Möglichkeit hinzufügen, dass bereits erstellte Task auch wieder verändert werden können. Dazu erstellen wir eine eigene EditTaskPage. Diese soll über einen Button auf der TaskDetailPage erreichbar sein

## EditTaskPageViewModel

Wir erstellen hierfür ein neues ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Models;
using MauiToDoApp.Services;

namespace MauiToDoApp.ViewModels;

[QueryProperty(nameof(SelectedTask), "Item")]
public partial class EditTaskPageViewModel : BaseViewModel
{
    private readonly DatabaseService _dbService;

    public EditTaskPageViewModel(DatabaseService dbService) => _dbService = dbService;

    [ObservableProperty]
    private TodoItem _selectedTask;

    [RelayCommand]
    private async Task Save()
    {
        await _dbService.SaveTaskAsync(SelectedTask);
        await Shell.Current.GoToAsync(".."); // Zurück zur Detailseite
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync("..");
}

```

## EditTaskPage.xaml

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiToDoApp.Pages.EditTaskPage"
             Title="EditTaskPage">
    <VerticalStackLayout Padding="20" Spacing="15">
        <Entry Text="{Binding SelectedTask.Title}" Placeholder="Titel der Aufgabe" />
        <Editor Text="{Binding SelectedTask.Description}" Placeholder="Beschreibung" HeightRequest="100" />

        <Label Text="Abgabedatum / Deadline:" FontAttributes="Bold" />
        <DatePicker Date="{Binding SelectedTask.DueDate}" Format="dd.MM.yyyy" />
        <Button Text="Speichern" Command="{Binding SaveCommand}" />
        <Button Text="Abbrechen" Command="{Binding CancelCommand}" BackgroundColor="Gray" />
    </VerticalStackLayout>
</ContentPage>
```

## Verbindung in der TaskDetailPage
Wir ergänzen in der TaskDetailPage einen "Bearbeiten"-Button.

```csharp
[RelayCommand]
private async Task NavigateToEdit() => 
    await Shell.Current.GoToAsync(nameof(EditTaskPage), new Dictionary<string, object> { { "Item", SelectedTask } });
```

XAML Ergänzung (in TaskDetailPage.xaml):

```xml
<Button Text="Bearbeiten" Command="{Binding NavigateToEditCommand}" Margin="0,20,0,0" />
```

## View und ViewModel registrieren

```csharp
builder.Services.AddTransient<EditTaskPage>();
builder.Services.AddTransient<EditTaskPageViewModel>();
```

## Route registrieren

```csharp
Routing.RegisterRoute(nameof(EditTaskPage), typeof(EditTaskPage));
```

# Uhrzeit hinzufügen
Damit man bei einem Task nicht nur das Datum mit einer immer fixen Uhrzeit wählen kann, werden wir jetzt beim erstellen und beim bearbeiten eines Tasks die Uhrzeit auch hinzufügen.

Wir fügen hierzu bei AddTaskPage und EditTaskPage einen Timepicker hinzu unf implementieren die Lokig im jeweiligen ViewModel

## AddTaskPage.xaml

```xml
<Label Text="Uhrzeit:" FontAttributes="Bold" />
    <TimePicker Time="{Binding DueTime}" />
```

## AddTaskPageViewModel.cs

Du benötigst ein zusätzliches TimeSpan Property für den TimePicker, da dieser nicht direkt mit einem DateTime Objekt gebunden werden kann. 

```csharp
[ObservableProperty] 
private TimeSpan _dueTime = DateTime.Now.TimeOfDay; 
```

Beim Speichern führen wir beides zusammen.

```csharp
[RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Hinweis", "Bitte Titel eingeben.", "OK");
            return;
        }

        // Datum und Zeit kombinieren
        DateTime finalDeadline = DueDate.Date + DueTime;

        var newTask = new TodoItem
        {
            Title = Title,
            Description = Description,
            IsDone = false,
            DueDate = finalDeadline // Hier das kombinierte Datum speichern
        };

        await _databaseService.SaveTaskAsync(newTask);

        // Benachrichtigung zum kombinierten Zeitpunkt
        await LocalNotificationCenter.Current.Show(new NotificationRequest
        {
            NotificationId = newTask.Id,
            Title = "HTL Deadline Erinnerung! 📝",
            Description = $"Die Abgabe für '{newTask.Title}' ist fällig!",
            Schedule = new NotificationRequestSchedule { NotifyTime = finalDeadline }
        });

        await Shell.Current.GoToAsync("..");
    }
```

## EditTaskPage
Damit der TimePicker auf der EditTaskPage korrekt mit deinem SelectedTask (dem TodoItem) funktioniert, musst du beachten, dass der TimePicker ein TimeSpan-Objekt benötigt, dein SelectedTask.DueDate aber ein DateTime ist.

Da SelectedTask ein komplexes Objekt ist, ist es im ViewModel am saubersten, wenn du die Zeit aus dem Datum extrahierst.

```xml
<Label Text="Uhrzeit:" FontAttributes="Bold" />
<TimePicker Time="{Binding SelectedTime}" />
```

```csharp
[QueryProperty(nameof(SelectedTask), "Item")]
public partial class EditTaskPageViewModel : BaseViewModel
{
    private readonly DatabaseService _dbService;

    public EditTaskPageViewModel(DatabaseService dbService) => _dbService = dbService;

    [ObservableProperty]
    private TodoItem _selectedTask;

    [ObservableProperty]
    private TimeSpan _selectedTime;

    [RelayCommand]
    private async Task Save()
    {
        // Kombiniere das Datum aus dem DatePicker mit der gewählten Zeit
        SelectedTask.DueDate = SelectedTask.DueDate.Date + SelectedTime;

        await _dbService.SaveTaskAsync(SelectedTask);
        await Shell.Current.GoToAsync(".."); // Zurück zur Detailseite
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync("..");
}
```

> [!WARNING]
> Die Uhrzeit wird zwar jetzt auf der Edit Page korrekt angezeigt und gespeichert, nur wenn wir dann wieder zur Details 
> zurück wechseln, wird die Uhrzeit nicht korrekt angezeigt

> [!NOTE]
> Das ist ein klassisches Problem bei der Datenbindung in .NET MAUI. Das Problem ist, dass die DetailsPage (oder ihr ViewModel) nicht weiß, 
> dass sich das Objekt im Hintergrund geändert hat, oder die DetailsPage beim Zurücknavigieren nicht neu geladen wurde.

## WeakReferenceMessenger
Da du das CommunityToolkit.Mvvm verwendest, hast du den WeakReferenceMessenger direkt an Bord. Damit kannst du die DetailsPage benachrichtigen, dass sich der Task geändert hat.

### Message erstellen
Erstelle einen neuen Ordner Messages und erstelle dort das File TaskUpdatedMessage.cs

```csharp
using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiToDoApp.Models;

namespace MauiToDoApp.Messages;

// Ein 'record' eignet sich perfekt als Nachrichtentyp
public class TaskUpdatedMessage : ValueChangedMessage<TodoItem>
{
    public TaskUpdatedMessage(TodoItem value) : base(value)
    {
    }
}
```

### EditTaskPageViewModel.cs
Sende nun die Nachricht über das Update an den Messanger

```csharp
[RelayCommand]
private async Task Save()
{
    // Kombiniere das Datum aus dem DatePicker mit der gewählten Zeit
    SelectedTask.DueDate = SelectedTask.DueDate.Date + SelectedTime;

    await _dbService.SaveTaskAsync(SelectedTask);

    // Sende eine Nachricht, dass ein Task aktualisiert wurde
    WeakReferenceMessenger.Default.Send(new TaskUpdatedMessage(SelectedTask));

    await Shell.Current.GoToAsync(".."); // Zurück zur Detailseite
}
```

### DetailsPageViewModel.cs
Registrier nun im Konstruktor dieses ViewModel als Empfänger dieser Message. Dadurch wird bei einer Änderung der SelectedTask neu gesetzt und dadurch die View auch über die Änderung verständigt

```csharp
public TaskDetailPageViewModel()
{
    WeakReferenceMessenger.Default.Register<TaskUpdatedMessage>(this, (r, m) =>
    {
        // Aktualisiere dein lokales Task-Objekt
        SelectedTask = m.Value;

        // Da die Property nicht komplett neu gesetzt wird, muss NotifyPropertyChanged explizit aufgerufen werden
        OnPropertyChanged(nameof(StatusText));
    });
}
```

# Zusammenführen von AddTaskPage und EditTaskPage
Diese beiden Pages und ihre ViewModels enthalten viel redundanten Code, daher werden wir diese beiden Pages zu einer (ManageTaskPage) 
zusammenführen

## ManageTaskPageViewModel.cs
Erstelle das neu ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiToDoApp.Messages;
using MauiToDoApp.Models;
using MauiToDoApp.Services;

[QueryProperty(nameof(TaskItem), "Item")]
public partial class ManageTaskPageViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    private TodoItem _taskItem = new();

    [ObservableProperty]
    private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;

    public ManageTaskPageViewModel(DatabaseService dbService) => _dbService = dbService;

    partial void OnTaskItemChanged(TodoItem value)
    {
        if (value != null)
            SelectedTime = value.DueDate.TimeOfDay;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(TaskItem.Title)) return;

        // Zeit und Datum zusammenführen
        TaskItem.DueDate = TaskItem.DueDate.Date + SelectedTime;

        await _dbService.SaveTaskAsync(TaskItem);

        // Falls wir bearbeiten, informiere andere Seiten
        if (TaskItem.Id != 0)
            WeakReferenceMessenger.Default.Send(new TaskUpdatedMessage(TaskItem));

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync("..");
}
```

## ManageTaskPage.xaml
Erstelle die View

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiToDoApp.Pages.ManageTaskPage"
             Title="ManageTaskPage">
    <VerticalStackLayout Padding="20" Spacing="15">
        <Entry Text="{Binding TaskItem.Title}" Placeholder="Titel der Aufgabe" />
        <Editor Text="{Binding TaskItem.Description}" Placeholder="Beschreibung" HeightRequest="100" />

        <Label Text="Abgabedatum / Deadline:" FontAttributes="Bold" />
        <DatePicker Date="{Binding TaskItem.DueDate}" Format="dd.MM.yyyy" />

        <Label Text="Uhrzeit:" FontAttributes="Bold" />
        <TimePicker Time="{Binding SelectedTime}" />

        <Button Text="Speichern" Command="{Binding SaveCommand}" />
        <Button Text="Abbrechen" Command="{Binding CancelCommand}" BackgroundColor="Gray" />
    </VerticalStackLayout>
</ContentPage>
```

## GoToAsync Methoden aktualisieren

MainPageViewModel.cs
```csharp
[RelayCommand]
private async Task GoToAddPage() => await Shell.Current.GoToAsync($"//{nameof(ManageTaskPage)}");
```

TaskDetailPageViewModel.cs
```csharp
[RelayCommand]
private async Task NavigateToEdit() =>
await Shell.Current.GoToAsync(nameof(ManageTaskPage), new Dictionary<string, object> { { "Item", SelectedTask } });
```

>[!NOTE]
> Vergiss nicht die Route, die Page und das ViewModel zu registrieren

## Löschen der AddTaskPage und EditTaskPage
Sobald die App getestet wurde, können wir die nun unnötigen Pages (AddTaskPage.xaml, EditTaskPage.xaml) und 
die ViewModels (AddTaskPageViewModel.cs und EditTaskPageViewModel.cs) löschen.

Lösche auch alle anderen Verweise auf die Files.

# Kategorien für ToDo Items
Im nächsten Schritt wollen wir Kategorien hinzufügen, sodass wir ToDo Items zu einer Kategorie hinzufügen können. Wir werden die Kategorien so implementieren, dass User auch selber Kategorien erstellen und bearbeiten können.

## Das Datenmodell für Kategorien
Wir benötigen eine Kategorie-Datenbank-Tabelle und eine 1-zu-n-Beziehung (ein TodoItem hat genau eine Category, eine Category hat viele TodoItems).

Erstelle im Ordner Models eine neues File Categories.cs

```csharp
public enum CategoryColor
{
    Blue,
    Green,
    Red,
    Yellow,
    Orange,
    Purple,
    Gray
}

// Models/Category.cs
public partial class Category : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private CategoryColor _colorType;
}
```

Füge die CategoryId als Fremdschlüssel im ToDoItem hinzu.
Damit dein TodoItem die Farbe halten kann, ohne dass sie in der SQLite-Datenbank gespeichert wird, nutzen wir das [Ignore] Attribut.

```csharp
public int CategoryId { get; set; } = 0;

[Ignore]
public CategoryColor CategoryColor { get; set; } = CategoryColor.Gray;
```

## DatabseService Für Categories
Nun müssen wir das DatabaseService so erweitern, dass wir Categories speicher, bearbeiten und löschen können.

Erstelle in det Init() die Tabelle Categories, falls diese nicht existiert
```csharp
await _database.CreateTableAsync<Category>();
```

Nun implementiere alle benötigten Methoden

```csharp
public async Task<List<Category>> GetCategoriesAsync()
{
    await Init();
    return await _database.Table<Category>().ToListAsync();
}

public async Task<Category?> GetCategoryByIdAsync(int id)
{
    await Init();
    return await _database.Table<Category>().FirstOrDefaultAsync(c => c.Id == id);
}

public async Task<int> SaveCategoryAsync(Category category)
{
    await Init();
    if (category.Id != 0)
        return await _database.UpdateAsync(category);
    else
        return await _database.InsertAsync(category);
}

public async Task<int> DeleteCategoryAsync(Category category)
{
    await Init();
    return await _database.DeleteAsync(category);
}
```

## Manage Categories
Nun erstellen wir die Pages damit der User die Categories managen kann

### Der Converter (Enum zu Color)
Wir müssen zuerst einen Converter erstellen, damit die UI die Hex-Farben aus der Datenbank versteht.
Erstelle einen neuen Ordner Common und darin ein neues File CategoryColorConverter.cs

```csharp
using MauiToDoApp.Models;
using System.Globalization;

namespace MauiToDoApp.Common;

public class CategoryColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CategoryColor colorType)
        {
            return colorType switch
            {
                CategoryColor.Blue => Colors.DeepSkyBlue,
                CategoryColor.Green => Colors.LimeGreen,
                CategoryColor.Red => Colors.Tomato,
                CategoryColor.Yellow => Colors.Gold,
                CategoryColor.Orange => Colors.Orange,
                CategoryColor.Purple => Colors.MediumPurple,
                CategoryColor.Gray => Colors.SlateGray,
                _ => Colors.Gray
            };
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
```

### Der Converter (Int zu Boolean)
Wir erstellen eine weiteren Converter (IntToBoolConverter.cs) der eine Int Wert in einen Boolean Wert umwandelt.
Er verwandelt den Int Wert 0 in FALSE und alle anderen in TRUE.
Diesen verwenden wir nachher um die Visibility des Löschen Buttons zu setzten (Dieser soll nur Visible sein wenn eine Kategorie editiert wird

```csharp
using System.Globalization;

namespace MauiToDoApp.Common;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int id && id != 0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

### ManageCategoryPageViewModel
Wir erstellen ein neues ViewModel um Categories hinzuzufügen und zu bearbeiten

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Models;
using MauiToDoApp.Services;

namespace MauiToDoApp.ViewModels;

[QueryProperty(nameof(Category), "Category")]
public partial class ManageCategoryPageViewModel : BaseViewModel
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    private Category _category = new();

    // Stellt alle Enum-Werte für den Picker bereit
    public List<CategoryColor> ColorTypes => Enum.GetValues(typeof(CategoryColor)).Cast<CategoryColor>().ToList();

    public ManageCategoryPageViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Category.Name)) return;

        await _dbService.SaveCategoryAsync(Category);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Category.Id == 0) return;
        await _dbService.DeleteCategoryAsync(Category);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync("..");
}
```

### ManageCategoryPageViewModel
Wir erstellen ein neues ViewModel zur Anzeige und Verwaltung der Categories.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:converter="clr-namespace:MauiToDoApp.Common"
             x:Class="MauiToDoApp.Pages.ManageCategoryPage"
             Title="ManageCategoryPage">
    <ContentPage.Resources>
        <converter:CategoryColorConverter x:Key="ColorConverter" />
        <converter:IntToBoolConverter x:Key="IntToBoolConverter" />
    </ContentPage.Resources>

    <VerticalStackLayout Padding="20" Spacing="15">

        <Label Text="Name der Kategorie:" FontAttributes="Bold" />
        <Entry Text="{Binding Category.Name}" Placeholder="z.B. Arbeit, Uni..." />

        <Label Text="Farbe wählen:" FontAttributes="Bold" />
        <HorizontalStackLayout Spacing="10">
            <Picker ItemsSource="{Binding ColorTypes}"
                SelectedItem="{Binding Category.ColorType}"
                WidthRequest="250" />
            <Border StrokeShape="RoundRectangle 10" 
                HeightRequest="40"
                WidthRequest="40"
                VerticalOptions="Center"
                BackgroundColor="{Binding Category.ColorType, Converter={StaticResource ColorConverter}}" />        
        </HorizontalStackLayout>

        <Button Text="Speichern" 
                Command="{Binding SaveCommand}" 
                Margin="0,20,0,0" />

        <Button Text="Löschen" 
                Command="{Binding DeleteCommand}" 
                BackgroundColor="Red"
                IsVisible="{Binding Category.Id, Converter={StaticResource IntToBoolConverter}}" />
        <Button Text="Abbrechen" 
                Command="{Binding CancelCommand}" 
                BackgroundColor="Gray" />

    </VerticalStackLayout>
</ContentPage>
```

Injecte nun das ViewModel
```xml
using MauiToDoApp.ViewModels;

namespace MauiToDoApp.Pages;

public partial class ManageCategoryPage : ContentPage
{
	public ManageCategoryPage(ManageCategoryPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
```

### CategoryListPageViewModel
Nun erstellen wir das ViewModel um eine Liste von allen Categories anzuzeigen. Die Seite soll es dem Beutzer zusätzlich ermöglichen, 
neue Kategorien hinzuzufügen, bzw Kategorien zu bearbeiten

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Models;
using MauiToDoApp.Pages;
using MauiToDoApp.Services;
using System.Collections.ObjectModel;

namespace MauiToDoApp.ViewModels;

public partial class CategoryListPageViewModel : BaseViewModel
{
    private readonly DatabaseService _dbService;

    public ObservableCollection<Category> Categories { get; } = new();

    public CategoryListPageViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        LoadCategories();
    }

    [RelayCommand]
    private async Task LoadCategories()
    {
        var list = await _dbService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var item in list) Categories.Add(item);
    }

    // Navigiert zur "leeren" Seite (Erstellen)
    [RelayCommand]
    private async Task AddNew() =>
        await Shell.Current.GoToAsync(nameof(ManageCategoryPage));

    // Navigiert mit Parameter zur Bearbeitungsseite
    [RelayCommand]
    private async Task EditCategory(Category category) =>
        await Shell.Current.GoToAsync(nameof(ManageCategoryPage), new Dictionary<string, object>
        {
            { "Category", category }
        });
}
```

### CategoryListPage
Nun erstellen wir die Page

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MauiToDoApp.ViewModels"
             xmlns:converter="clr-namespace:MauiToDoApp.Common"
             x:Class="MauiToDoApp.Pages.CategoryListPage"
             Title="CategoryListPage">
    <ContentPage.Resources>
        <converter:CategoryColorConverter x:Key="ColorConverter" />
    </ContentPage.Resources>

    <Grid RowDefinitions="*, Auto" Padding="15">
        <CollectionView ItemsSource="{Binding Categories}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Border Margin="0,5" Padding="15" StrokeShape="RoundRectangle 10">
                        <Border.GestureRecognizers>
                            <TapGestureRecognizer 
                                Command="{Binding Source={RelativeSource AncestorType={x:Type vm:CategoryListPageViewModel}}, Path=EditCategoryCommand}" 
                                CommandParameter="{Binding .}" />
                        </Border.GestureRecognizers>

                        <HorizontalStackLayout Spacing="15">
                            <BoxView Color="{Binding ColorType, Converter={StaticResource ColorConverter}}" 
                                     WidthRequest="20" HeightRequest="20" CornerRadius="10" />
                            <Label Text="{Binding Name}" VerticalOptions="Center" FontAttributes="Bold" />
                        </HorizontalStackLayout>
                    </Border>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <Button Grid.Row="1" Text="Neue Kategorie hinzufügen" 
                Command="{Binding AddNewCommand}" 
                Margin="0,10,0,0" />
    </Grid>
</ContentPage>
```

Injecte nun das ViewModel

```xml
using MauiToDoApp.ViewModels;

namespace MauiToDoApp.Pages;

public partial class CategoryListPage : ContentPage
{
	public CategoryListPage(CategoryListPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
```

>[!NOTE]
> Vergiss nicht die Routen, die Pages und das ViewModels zu registrieren


### Neuer Tab
Nun müssen wir nur noch in AppShell.xaml einen neuen Tab in der Tabbar hinzufügen, damit der Benutzer auch auf die Category Page kommt

```xml
<Tab Title="Manage" Icon="manage_icon.png">
    <ShellContent 
ContentTemplate="{DataTemplate pages:CategoryListPage}" 
Route="CategoryListPage" />
</Tab>
```

>[!WARNING]
> Wie schon in einem früheren Teil bekommt die View nicht mit, dass bei Delete die Liste der Categories geändert wurde 
> und daher von der View neu geladen werden muss.

Erstelle nun eigenständig eine neue Message und zwinge das ViewModel dazu die Liste neu zu laden

<details>
<summary>Message</summary>

```csharp
namespace MauiToDoApp.Messages;

public class CategoriesUpdatedMessage { }
```

</details>

<details>
<summary>ManageCategoryPageViewModel</summary>

```csharp
private async Task Save()
{
    if (string.IsNullOrWhiteSpace(Category.Name)) return;
    await _dbService.SaveCategoryAsync(Category);
    WeakReferenceMessenger.Default.Send(new CategoriesUpdatedMessage());
    await Shell.Current.GoToAsync("..");
}

[RelayCommand]
private async Task Delete()
{
    if (Category.Id == 0) return;
    await _dbService.DeleteCategoryAsync(Category);
    WeakReferenceMessenger.Default.Send(new CategoriesUpdatedMessage());
    await Shell.Current.GoToAsync("..");
}
```
</details>

<details>
<summary>CategoryListPageViewModel</summary>

```csharp
public CategoryListPageViewModel(DatabaseService dbService)
{
    _dbService = dbService;
    LoadCategories();

    WeakReferenceMessenger.Default.Register<CategoriesUpdatedMessage>(this, (r, m) =>
    {
        MainThread.BeginInvokeOnMainThread(async () => await LoadCategories());
    });
}
```

</details>

# Categories in die View integrieren
Als nächstes müssen wir die Kategorien in die View integrieren, so dass man sie bei den ToDo Items sieht.

## MainPage
Als erstes passen wir die MainPage so an, dass die ToDoItems in der Lsite die Farbe der Kategorie anzeigen

### MainPageViewModel.cs
Hier müssen wir lediglich die LoadTaskAsync() Methode so anpassen, dass im Task auch die Kategorie hinzugefügt wird. Dies geschieht, da in unserer Datenbank nur die ID der Kategorie gespeichert wird und nicht die Farbe

```csharp
private async Task LoadTasksAsync()
{
    var items = await _dbService.GetTasksAsync();
    var categories = await _dbService.GetCategoriesAsync();
    Tasks.Clear();
    foreach (var item in items)
    {
        var category = categories.FirstOrDefault(c => c.Id == item.CategoryId);
        item.CategoryColor = category?.ColorType ?? CategoryColor.Gray;

        Tasks.Add(item);
    }
}
```

### MainPage.xaml
In der MainPage zeigen wir vor jedem ToDoItem eine kleine Box (BoxView) mit der Farbe der Kategorie an. 
Dazu verwenden wir wieder den Converter um auf das Enum zu binden.

```xml
<ContentPage.Resources>
    <converter:CategoryColorConverter x:Key="ColorConverter" />
</ContentPage.Resources>

...

<HorizontalStackLayout Spacing="15" Padding="10">
    <BoxView Color="{Binding CategoryColor, Converter={StaticResource ColorConverter}}" 
            WidthRequest="10" CornerRadius="5" />
    <CheckBox IsChecked="{Binding IsDone}"
            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:MainPageViewModel}}, Path=ToggleTaskStatusCommand}"
            CommandParameter="{Binding .}" />
    <Label Text="{Binding Title}" VerticalOptions="Center" FontSize="18" />
</HorizontalStackLayout>

...
```

## ManageTask
Nun müssen wir die ManageTask Page so anpassen, dass beim Hinzufügen eines neuen ToDoItems die Kategorie ausgewählt werden kann, 
bzw beim Ändern, die Kategorie geändert werden kann.

### ManageTaskPageViewModel
Wir müssen das ViewModel so anpassen, dass die Kategorie ausgewählt werden kann.

Wir brauchen dafür 2 neue Properties. Eine für die Liste der verfügbaren Kategorien und die andere für die ausgewählte Kategorie
```csharp
public ObservableCollection<Category> Categories { get; } = new();

[ObservableProperty]
private Category? _selectedCategory;
```

Nun passen wir den Konstruktor an und implementieren eine Methode die alle verfügbaren Kategorien aus der Datenbank läd
```csharp
public ManageTaskPageViewModel(DatabaseService dbService)
{
    _dbService = dbService;
    LoadCategories();
}

// NEU
private async void LoadCategories()
{
    var list = await _dbService.GetCategoriesAsync();
    Categories.Clear();
    foreach (var cat in list)
    {
        Categories.Add(cat);
    }

    // Wenn wir bearbeiten, Kategorie anhand der ID setzen
    if (TaskItem.CategoryId != 0)
    {
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == TaskItem.CategoryId);
    }
}
```

Wir implementieren die Methode OnTaskItemChanged damit bei einer bereits ausgewählten Kategorie (ToDoItem bearbeiten) 
gleich die Richtige Kategorie und Farbe angezeigt wird 

```csharp
partial void OnTaskItemChanged(TodoItem value)
{
    if (value != null)
    {
        SelectedTime = value.DueDate.TimeOfDay;
        // Falls Categories schon geladen sind, direkt selektieren
        if (Categories.Any())
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
        }
    }
}
```

Wir erweitern die Save() Methode, damit direkt nach dem Datum und Uhrzeit auch die Kategorie zugewiesen wird

```csharp
// Zeit und Datum zusammenführen
TaskItem.DueDate = TaskItem.DueDate.Date + SelectedTime;
// Kategorie zuweisen
TaskItem.CategoryId = SelectedCategory?.Id ?? 0;
```

### ManageTaskPage
Nun passen wir die View an, sodass die Kategorie und die Farbe angezeigt werden

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:converter="clr-namespace:MauiToDoApp.Common"
             x:Class="MauiToDoApp.Pages.ManageTaskPage"
             Title="ManageTaskPage">
    <ContentPage.Resources>
        <converter:CategoryColorConverter x:Key="ColorConverter" />
    </ContentPage.Resources>
    ...
        <Label Text="Kategorie wählen:" FontAttributes="Bold" />

        <HorizontalStackLayout Spacing="10">
            <Picker ItemsSource="{Binding Categories}"
            ItemDisplayBinding="{Binding Name}"
            SelectedItem="{Binding SelectedCategory}"
            WidthRequest="250" />

            <Border StrokeShape="RoundRectangle 10" 
            HeightRequest="40"
            WidthRequest="40"
            VerticalOptions="Center"
            BackgroundColor="{Binding SelectedCategory.ColorType, Converter={StaticResource ColorConverter}}" />
        </HorizontalStackLayout>
    ...
</ContentPage>
```

### MainPage
Registriere nun im Konstruktor die Message TaskUpdatedMessage damit die Liste der tasks neu geladen und 
die View bei der Änderung von Tasks benachrichtigt wird

```csharp
public MainPageViewModel(DatabaseService dbService)
{
    _dbService = dbService;
    Title = "HTL Aufgaben";
    LoadTasksAsync();

    WeakReferenceMessenger.Default.Register<TaskUpdatedMessage>(this, (r, m) =>
    {
        // Aktualisiere die Task Liste
        LoadTasksAsync();
    });
}
````

## TaskDetail
Nun müssen wir nur noch die TaskDetailPage anpassen, damit auch dort die Kategorie und die Farbe angezeigt wird

### TaskDetailPageViewModel
Wir benötigen eine neue Property damit die View die Daten für die Kategorie bekommt

```csharp
[ObservableProperty]
private Category _selectedCategory;
```

Jedes mal wenn sich der Task ändert, müssen wir die Kategorie neu aus der Datenbank laden. 

```csharp
partial void OnSelectedTaskChanged(TodoItem value)
{
    _ = RefreshCategoryAsync();
}

private async Task RefreshCategoryAsync()
{
    if (SelectedTask != null && SelectedTask.CategoryId != 0)
        SelectedCategory = await _dbService.GetCategoryByIdAsync(SelectedTask.CategoryId);
    else
        SelectedCategory = null;
}
```

Jetzt müssen wir den Konstruktor noch anpassen, sodass wir bei jeder Änderung des Tasks, auch die Kategorie neu laden

```csharp
public TaskDetailPageViewModel(DatabaseService dbService)
{
    _dbService = dbService;

    WeakReferenceMessenger.Default.Register<TaskUpdatedMessage>(this, (r, m) =>
    {
        SelectedTask = m.Value;
        OnPropertyChanged(nameof(StatusText));
        RefreshCategoryAsync();
    });
}
```

### TaskDetailPage
Hier fügen wir nur noch den Kategorie Namen und die Farbe hinzu

```xml
<Label Text="Kategorie:" FontAttributes="Bold" />
<HorizontalStackLayout Spacing="10">
    <Label Text="{Binding SelectedCategory.Name, TargetNullValue='Keine Kategorie vorhanden.'}" FontSize="16" WidthRequest="250" />
    <Border StrokeShape="RoundRectangle 10" 
        HeightRequest="40"
        WidthRequest="40"
        VerticalOptions="Center"
        BackgroundColor="{Binding SelectedCategory.ColorType, Converter={StaticResource ColorConverter}}" />
</HorizontalStackLayout>
```

# Logging einbauen
Für jede App ist es wichtig eine Logging Funktion zu integrieren.
Für unsere App bauen wir nun ein eigenes Logging Service.
Diese Loggt für uns Fehler mit und schreibt diese in ein File.
Um bequem auf das File zugreifen zu können, erstellen wir zusätzlich eine eigene Page und verlinken diese in der Tabbar.

## FileLogger
Erstelle im Ordner Services eine neues File FileLogger.cs.
Hier implementieren die Funktionalität um Logs in eine File zuschreiben, dieses File zu lesen und es auch wieder zu löschen.

```csharp
namespace MauiToDoApp.Services;

public static class FileLogger
{
    private static readonly string _logPath = Path.Combine(
        FileSystem.AppDataDirectory, "app_log.txt");

    public static async Task LogAsync(string message, Exception? ex = null)
    {
        try
        {
            var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            if (ex != null)
                entry += $"\n  >> {ex.GetType().Name}: {ex.Message}" +
                         $"\n  >> {ex.StackTrace}";

            await File.AppendAllTextAsync(_logPath, entry + "\n\n");
        }
        catch { /* Logger darf nie crashen */ }
    }

    public static async Task<string> ReadLogsAsync()
    {
        if (!File.Exists(_logPath)) return "Keine Logs vorhanden.";
        return await File.ReadAllTextAsync(_logPath);
    }

    public static void Clear()
    {
        if (File.Exists(_logPath)) File.Delete(_logPath);
    }

    public static string LogPath => _logPath;
}
```

Ansich wäre damit die Funktionalität für den Logger fertig, allerdings wollen wir ja für unsere App eine eigene Page erstellen,
in der wir uns die mitgeloggten Einträge anschauen können.

## LogPageViewModel
Nun erstellen wir ein ViewModel für unsere LogPage.
Wir stellen der View den Inhalt des Log Files zur Verfügung, ermöglichen den Log zu teilen, bzw diesen auch wiedre zu löschen

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiToDoApp.Services;

namespace MauiToDoApp.ViewModels;

public partial class LogPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _logContent = "Lade...";

    public string LogPath => FileLogger.LogPath;

    public LogPageViewModel()
    {
        _ = LoadLogs();
    }

    [RelayCommand]
    private async Task Refresh() => await LoadLogs();

    [RelayCommand]
    private void Clear()
    {
        FileLogger.Clear();
        LogContent = "Logs gelöscht.";
    }

    [RelayCommand]
    private async Task Share()
    {
        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(
            new ShareFileRequest
            {
                Title = "App Log",
                File = new ShareFile(FileLogger.LogPath)
            });
    }

    private async Task LoadLogs()
    {
        LogContent = await FileLogger.ReadLogsAsync();
    }
}
```

## LogPage
Nun erstellen wir fürs Logging eine neue Page

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MauiToDoApp.ViewModels"
             x:Class="MauiToDoApp.Pages.LogPage"
             Title="Debug Logs">

    <ContentPage.BindingContext>
        <vm:LogPageViewModel />
    </ContentPage.BindingContext>

    <Grid RowDefinitions="Auto, *, Auto" Padding="10" RowSpacing="10">

        <HorizontalStackLayout Grid.Row="0" Spacing="10">
            <Button Text="Aktualisieren" Command="{Binding RefreshCommand}" />
            <Button Text="Löschen" Command="{Binding ClearCommand}" BackgroundColor="Red" />
            <Button Text="Teilen" Command="{Binding ShareCommand}" />
        </HorizontalStackLayout>

        <ScrollView Grid.Row="1">
            <Label Text="{Binding LogContent}"
                   FontFamily="Monospace"
                   FontSize="11"
                   Padding="5" />
        </ScrollView>

        <Label Grid.Row="2"
               Text="{Binding LogPath}"
               FontSize="10"
               TextColor="Gray" />
    </Grid>
</ContentPage>
```

Vergiss nicht das ViewModel zu injecten

```csharp
public LogPage(LogPageViewModel vm)
{
	InitializeComponent();
	BindingContext = vm;
}
```

## Global Exception Handler
Der Global Exception Handler fängt alle Exceptions ab, die nirgendwo sonst gecatcht wurden und normalerweise die App zum Absturz bringen würden.

In App.xaml.cs
```csharp
public App()
{
    InitializeComponent();

    AppDomain.CurrentDomain.UnhandledException += async (s, e) =>
        await FileLogger.LogAsync("UnhandledException", e.ExceptionObject as Exception);

    TaskScheduler.UnobservedTaskException += async (s, e) =>
    {
        await FileLogger.LogAsync("UnobservedTaskException", e.Exception);
        e.SetObserved();
    };
}
```

## Log Tab hinzufügen
Füge nun in AppShell.xaml einen Tab hinzu

```xml
<Tab Title="Logs" Icon="bug.png">
    <ShellContent ContentTemplate="{DataTemplate pages:DebugLogPage}" />
</Tab>
```

## LogPage Route registrieren
Registriere nun in AppShell.xaml.cs die Route zur LogPage

```csharp
Routing.RegisterRoute(nameof(LogPage), typeof(LogPage));
```

## DI registrieren
Nun registrieren wir vie Page und das ViewModel für DI

```csharp
builder.Services.AddTransient<LogPage>();
builder.Services.AddTransient<LogPageViewModel>();
```

# Suche, Filter und Sortierung hinzufügen
Wir wollen nun die Usability der App verbessern und die Suche, Sortierung und Filterung der ToDo Items hinzufügen

## Suche
Um die Suche zu integrieren brauchen wir im xaml File eien Searchbar und im ViewModel eine Funktion die 
den Titel und die Beschreibung nach dem gesuchten Wort filtert

### MainPage.xaml
Wir passen als erstes das Grid an und fügen 2 weitere Rows hinzu.

>[!NOTE]
> Vergiss nicht Grid.Row für alle Elemente anzupassen, da wir am Anfang der Seite eine Row hinzufügen

```xml
<Grid RowDefinitions="Auto, *, Auto" Padding="20">
```

Füge nun die Searchbar hinzu

```xml
<SearchBar Grid.Row="0" Text="{Binding SearchText}" Placeholder="Titel oder Beschreibung suchen..." />
```

### MainPageViewModel.cs
Damit die Liste durchsucht werden kann brauchen wir eine neue ObservableProperty auf die die Searchbar binden kann.
Wir fügen zusätzlich einen Callback hinzu, der bei jeder Änderung des Textes in der Searchbar aufgerufen wird.
Zuletzt brauchen wir eine Methode, die unsere ToDo Items filtert

Um die neuen Funktionalität optimal zu integrieren, müssen wir unser ViewModel umbauen.

Füge eine neue Liste hinzu, die die Rohdaten der ToDo Items enthält

```csharp
private List<TodoItem> _allItems = new();
```

Füge eine ObservableProperty hinzu auf die der Searchtext der View binden kann
```csharp
[ObservableProperty] string _searchText;
```

Implementier eine Methode die unsere Suche und im Anschluss die Filterung durchführt

```csharp
private void ApplyFiltersAndSort()
{
    var filtered = _allItems.AsEnumerable();

    // 1. Suche über Title und Description
    if (!string.IsNullOrWhiteSpace(SearchText))
        filtered = filtered.Where(i =>
            i.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            i.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            
    Tasks.Clear();
    foreach (var item in filtered) Tasks.Add(item);
}
```

Passe nun die LoadTasksAsync() Methode so an, dass erst die Rohdaten geladen und dann die Suche und Filter auf die Daten angewendet werden.

```csharp
private async Task LoadTasksAsync()
{
    var items = await _dbService.GetTasksAsync();
    var categories = await _dbService.GetCategoriesAsync();
            
    // Rohdaten vorbereiten
    _allItems = items.Select(item => {
        var cat = categories.FirstOrDefault(c => c.Id == item.CategoryId);
        item.CategoryColor = cat?.ColorType ?? CategoryColor.Gray;
        return item;
    }).ToList();

    ApplyFiltersAndSort();
}
```

Füge nun die Callback Methode hinzu, die ausgeführt wird, sobald sich der Text in der Searchbar ändert.

```csharp
partial void OnSearchTextChanged(string value) => ApplyFiltersAndSort();
```

>[!NOTE]
> Die Methode OnSearchTextChanged wird automatisch durch die [ObservableProperty] string _searchText generiert.

## Filtern und sortieren
Im nächsten Schritt fürgen wir den Filter hinzu, sodass nach verfügbaren Kategorien gefiltert werden kann.

### MainPage.xaml
Erweitere die MainPage umd eine weitere Row um den Filter anzuzeigen.

>[!NOTE]
> Vergiss nicht Grid.Row für alle Elemente anzupassen, da wir am Anfang der Seite eine Row hinzufügen

```xml
<Grid RowDefinitions="Auto, Auto, *, Auto" Padding="20">
```

Füge nun nach der Searchbar ein HorizontalStackLayout mit dem Filter hinzu

```xml
<HorizontalStackLayout Grid.Row="1" Spacing="10" Margin="0,10">
    <Picker Title="Kategorie" SelectedItem="{Binding SelectedCategory}" 
        ItemsSource="{Binding AvailableCategories}" WidthRequest="150" />            
</HorizontalStackLayout>
```

### MainPageViewModel.cs
Für den Filter brauchen wir eine ObservableCollection mit allen verfügbaren Kategorien.

```csharp
public ObservableCollection<Category> AvailableCategories { get; } = new();
```

Füge nun eine ObservableProperty hinzu, die auf selektierte Kategorie aus dem Dropdown bindet.

```csharp
[ObservableProperty] Category _selectedCategory;
```

Erweitere nun die LoadTaskAsync Methode, um die Kategorien für den Filter vorzubereiten

```csharp
private async Task LoadTasksAsync()
{
    var items = await _dbService.GetTasksAsync();
    var categories = await _dbService.GetCategoriesAsync();

    // Kategorien für den Filter vorbereiten
    AvailableCategories.Clear();
    AvailableCategories.Add(new Category { Id = 0, Name = "Alle" }); 
    foreach (var cat in categories) AvailableCategories.Add(cat);

    // Rohdaten vorbereiten
    _allItems = items.Select(item => {
        ...
    }
}
```

Jetzt passen wir die Methode ApplyFilterAndSort() noch so an, dass nach Kategorien gefiltert wird.
Implentiere den Teil direkt nach der Suche

```csharp
// 2. Filter nach Kategorie (Jetzt über das Objekt!)
if (SelectedCategory != null && SelectedCategory.Id != 0)
{
    filtered = filtered.Where(i => i.CategoryId == SelectedCategory.Id);
}
```

Füge nun die Callback Methode hinzu, die ausgeführt wird, sobald sich die ausgewählte Kategorie ändert.
```csharp
partial void OnSelectedCategoryChanged(Category value) => ApplyFiltersAndSort();
```

## Sortierung
Zuletzt fügen wir noch die Sortierung hinzu. 
Die Default Sortierung ist nach dem Titel. Zusatzlich soll noch nach Fälligkeitsdatum und Kategorie sortiert werden können.

### MainPage.xaml
Füge nach dem Dropdown (Picker) für die Kategorien, das Dropdown für die Sortierung hinzu

```xml
<Picker Title="Sortieren nach" 
    ItemsSource="{Binding SortOptions}" 
    SelectedItem="{Binding SelectedSortOption}"
    ItemDisplayBinding="{Binding DisplayName}" 
    WidthRequest="150" />
```

### MainPageViewModel.cs
Im ViewModel definieren wir eine einfache Hilfsklasse (oder ein Record) die die Art der Sortierung enthält
Diese muss außerhalb der Klasse definiert werden.

```csharp
public record SortOption(string DisplayName, string Key);
```

In der Klasse initialisieren wir die Collection mit den Sortieroptionen für das Dropdown.
Wir definieren zusätzlich eine ObservableProperty, die die ausgewäjlte sortierung enthält.

```csharp
public ObservableCollection<SortOption> SortOptions { get; } = new()
{
    new("Titel", "Title"),
    new("Fälligkeitsdatum", "DueDate")
};

// Das aktuell ausgewählte Sortierung
[ObservableProperty] 
private SortOption _selectedSortOption;
```

Im Konstruktor soll standarmäßig die erste Sortierung ausgewählt werden.

```csharp
// Standardmäßig die erste Option wählen
SelectedSortOption = SortOptions.First();
```

In der Methide ApplyFiltersAndSort fügen wir nach der Filterung, die Sortierung hinzu

```csharp
// 3. Sortierung basierend auf dem Key
filtered = SelectedSortOption.Key switch
{
    "DueDate" => filtered.OrderBy(i => i.DueDate),
    _ => filtered.OrderBy(i => i.Title)
};
```

Füge nun die Callback Methode hinzu, die ausgeführt wird, sobald sich die ausgewählte Sortierung ändert.
```csharp
partial void OnSelectedSortOptionChanged(SortOption value) => ApplyFiltersAndSort();
```

# Design der Seite
Im nächsten Schritt wollen wir die App etwas schöner gestalten.
Dafür definieren wir für alle Controls eigene Styles in einem RessourceDictionary und wende diese dann auf die Controls an.

## ResourceDictionary
Kopiere folgende Styles in das Styles.xaml ResourceDictionary (Resources->Styles->Styles.xaml)

```xml
<Color x:Key="PrimaryBrand">#16417C</Color>
<Color x:Key="CardBackground">#FFFFFF</Color>
<Color x:Key="AppBackground">#FDF9F3</Color>
<Color x:Key="TextSecondary">#636E72</Color>
<Color x:Key="TextPrimary">#FFFFFF</Color>
<Color x:Key="ShellInactive">#B2BEC3</Color>
<Color x:Key="ShellText">#2D3436</Color>

<Style x:Key="DolyShellStyle" TargetType="Shell">
    <Setter Property="Shell.BackgroundColor" Value="{StaticResource PrimaryBrand}" />
    <Setter Property="Shell.TabBarBackgroundColor" Value="{StaticResource PrimaryBrand}" />
    <Setter Property="Shell.TabBarForegroundColor" Value="{StaticResource TextPrimary}" />
    <Setter Property="Shell.TabBarUnselectedColor" Value="{StaticResource ShellInactive}" />
    <Setter Property="Shell.TitleColor" Value="{StaticResource ShellText}" />
</Style>

<Style x:Key="DolyButtonStyle" TargetType="Button">
    <Setter Property="CornerRadius" Value="25" />
    <Setter Property="HeightRequest" Value="50" />
    <Setter Property="BackgroundColor" Value="{StaticResource PrimaryBrand}" />
    <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="FontSize" Value="16" />
</Style>

<Style x:Key="DolyCardStyle" TargetType="Border">
    <Setter Property="StrokeShape" Value="RoundRectangle 15" />
    <Setter Property="BackgroundColor" Value="{StaticResource CardBackground}" />
    <Setter Property="Stroke" Value="Transparent" />
    <Setter Property="Padding" Value="15" />
    <Setter Property="Margin" Value="0,5" />
    <Setter Property="Shadow">
        <Shadow Brush="Black" Radius="5" Opacity="0.1" />
    </Setter>
</Style>

<Style x:Key="DolyLabelStyle" TargetType="Label">
    <Setter Property="TextColor" Value="{StaticResource TextSecondary}" />
    <Setter Property="FontSize" Value="16" />
</Style>

<Style x:Key="DolyBoxViewStyle" TargetType="BoxView">
    <Setter Property="WidthRequest" Value="8" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="VerticalOptions" Value="Fill" />
</Style>

<Style x:Key="DolyCheckBoxStyle" TargetType="CheckBox">
    <Setter Property="Color" Value="{StaticResource PrimaryBrand}" />
</Style>

<Style x:Key="DolyCollectionStyle" TargetType="CollectionView">
    <Setter Property="Margin" Value="0,10" />
</Style>

<Style x:Key="DolyEntryStyle" TargetType="Entry">
    <Setter Property="BackgroundColor" Value="White" />
    <Setter Property="TextColor" Value="{StaticResource PrimaryBrand}" />
    <Setter Property="HeightRequest" Value="50" />
    <Setter Property="Margin" Value="0,5" />
</Style>

<Style x:Key="DolyPickerStyle" TargetType="Picker">
    <Setter Property="BackgroundColor" Value="White" />
    <Setter Property="TextColor" Value="{StaticResource PrimaryBrand}" />
    <Setter Property="HeightRequest" Value="70" />
    <Setter Property="TitleColor" Value="Gray" />
</Style>
```

## Styling der Pages
Wir wenden jetzt auf alle Control die im Dictionary einen Style haben, diesen Style an

Hier einzelne Beispiele aus MainPage.xaml

```diff
<Entry Grid.Row="0" 
+      Style="{StaticResource DolyEntryStyle}"
-      FontSize="20" 
-      FontAttributes="Bold" 
-      HorizontalTextAlignment="Center" 
-      TextColor="{AppThemeBinding Light=#512BD4, Dark=#A294F9}"
       Text="{Binding SearchText}" 
       Placeholder="Titel oder Beschreibung suchen..." />
...
<Picker Title="Kategorie" 
+   Style="{StaticResource DolyPickerStyle}"
    ItemsSource="{Binding AvailableCategories}" 
    SelectedItem="{Binding SelectedCategory}"
    ItemDisplayBinding="{Binding Name}" 
    WidthRequest="150" />
...
<CollectionView Grid.Row="2"
+               Style="{StaticResource DolyCollectionStyle}"
                ItemsSource="{Binding Tasks}"
                SelectionMode="Single"
                SelectionChangedCommand="{Binding NavigateToDetailsCommand}"
                SelectionChangedCommandParameter="{Binding Source={RelativeSource Self}, Path=SelectedItem}">
...
```

Style in der Art alle Pages und entferne etwaige inline Styles die wir jetzt nichtmehr brauchen.

# Routing
Das Routing in .NET MAUI Shell kann bei komplexen Apps verwirrend sein, da es zwei verschiedene "Ebenen" der Navigation gibt. 

## Die zwei Navigations-Welten
Der Shell-Stack (Hierarchisch): Wenn du GoToAsync("DetailSeite") machst, "stapelst" du Seiten übereinander. Der ..-Befehl funktioniert hier perfekt, um eine Ebene zurückzugehen.

Die Tab-Ebene (Root): Wenn du dich in einem Tab (z.B. Aufgabenliste) befindest, ist das die Basis deiner App. Ein .. "nach oben" führt ins Leere, da du bereits am Boden des Stacks bist.

## Die "Safe-Navigation"
Um aus jedem Kontext (egal ob du von einem Log-Tab, einer Detailseite oder sonst wo kommst) sicher auf die Startseite zu gelangen, ist die Absolute Route mit dem //-Operator die einzig verlässliche Lösung.

Hier ein Beispiel für den Cancel Button

```csharp
private async Task Cancel()
{
    // 1. Prüfe, ob wir im aktuellen Tab tiefer in einer Hierarchie sind
    // Wenn der Stack größer als 1 ist, gab es eine Push-Navigation
    if (Shell.Current.Navigation.NavigationStack.Count > 1)
    {
        await Shell.Current.GoToAsync("..");
    }
    // 2. Ansonsten springe hart zurück zum Root-Tab (MainPage)
    else
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}
```