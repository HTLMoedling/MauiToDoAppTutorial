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

Update TaskDetailPage.xaml
```xml
<Label x:Name="StatusLabel" FontSize="14" FontAttributes="Italic" TextColor="Gray" />
```

Update Methode UpdateUI() in TaskDetailPage.xaml.cs
```csharp
StatusLabel.Text = SelectedTask.IsDone 
            ? "Status: Erledigt ✅" 
            : $"Status: Offen (Fällig am: {SelectedTask.DueDate.ToString("dd.MM.yyyy")}) ⏳";
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
