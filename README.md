# Task Management System

ASP.NET Core MVC web app for the C# course project: tasks with status, priority, deadlines, CRUD, validation, error handling, reporting, and **SignalR** real-time notifications.

## Requirements mapping (course rubric)

| Requirement | Implementation |
|-------------|----------------|
| C# / .NET 6+ | .NET 8, C# 12 |
| ASP.NET Core | MVC, Razor views, Bootstrap 5 |
| Database + EF Core | SQLite file `taskmanagement.db`, `AppDbContext`, async queries |
| CRUD | `TasksController` + views (Create, Read, Update, Delete) |
| UI | Layout, dashboard, task list with status filter, reports page |
| Input validation | `DataAnnotations` on `TaskItemInputModel`, client-side jQuery unobtrusive validation |
| Error handling | Development: developer exception page; Production: `/Home/Error` with `IExceptionHandlerFeature` |
| Simple reporting | Dashboard (`Home/Index`) and **Reports** (`Reports/Index`): counts by status, overdue list, due this week |
| Emerging technology | **SignalR** — `NotificationHub`, toasts + notification dropdown; background reminders for due/overdue tasks |
| Git + GitHub | Initialize repo locally (see below), add remote, push |

## Object-oriented design

- **Entities:** `TaskItem`, `TaskItemStatus`, `TaskPriority`.
- **Data access:** `AppDbContext` (EF Core).
- **Services:** `ITaskService` / `TaskService` (CRUD + reporting), `INotificationBroadcaster` / `NotificationBroadcaster` (SignalR).
- **Hosted service:** `DeadlineNotificationBackgroundService` scans for upcoming/overdue tasks and broadcasts notifications.

## Run locally

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd TaskManagement.Web
dotnet restore
dotnet run
```

Open the URL shown (default profile uses `http://localhost:5075`). The database is created on first run (`EnsureCreated`) and sample tasks are seeded if the database is empty.

### Configuration

- Connection string: `appsettings.json` → `ConnectionStrings:DefaultConnection`.
- Deadline checker: `DeadlineReminder:CheckIntervalMinutes`, `DeadlineReminder:HoursBeforeDue`.

### Schema changes

This sample uses `EnsureCreated()` for simplicity. If you change the model, delete `taskmanagement.db` and restart, or switch to EF Core migrations (`dotnet ef migrations add ...` / `dotnet ef database update`).

## GitHub

From the repository root:

```bash
git init
git add .
git commit -m "Initial commit: Task Management System"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git
git push -u origin main
```

Replace `YOUR_USERNAME/YOUR_REPO` with your GitHub repository.

## Project structure (main)

- `TaskManagement.Web/Controllers` — MVC controllers
- `TaskManagement.Web/Data` — `AppDbContext`, `SeedData`
- `TaskManagement.Web/Hubs` — `NotificationHub`
- `TaskManagement.Web/Models` — domain + view models
- `TaskManagement.Web/Services` — business logic and background notifications
- `TaskManagement.Web/Views` — Razor UI
- `TaskManagement.Web/wwwroot` — CSS and SignalR client script

## License

Educational / hobby project.
