<p align="center">
  <strong>Arcadia.Notifications</strong><br>
  <em>Toast notifications for Blazor — fire and forget</em>
</p>

## Quick Start

```bash
dotnet add package Arcadia.Notifications
```

```csharp
// Program.cs
builder.Services.AddScoped<ToastService>();
```

```html
<link href="_content/Arcadia.Notifications/css/arcadia-notifications.css" rel="stylesheet" />
```

```razor
<!-- In your layout -->
<ArcadiaToastContainer Position="ToastPosition.TopRight" />
```

Then anywhere in your app:

```csharp
@inject ToastService Toast

Toast.ShowSuccess("Saved!");
Toast.ShowError("Something went wrong.");
Toast.ShowWarning("Check your input.");
Toast.ShowInfo("New update available.");
```

## Features

- **4 severity levels** — Success, Error, Warning, Info with distinct colors
- **Auto-dismiss** — configurable duration, or persist until clicked
- **Stacking** — multiple toasts stack vertically with smooth animations
- **Positioning** — TopRight, TopLeft, BottomRight, BottomLeft, TopCenter, BottomCenter
- **Custom titles** — `Toast.ShowSuccess("Saved!", "Changes Applied")`
- **Dismiss all** — `Toast.DismissAll()`

**[Docs](https://arcadiaui.com/docs/notifications)** · **[GitHub](https://github.com/ArcadiaUIDev/arcadia)**
