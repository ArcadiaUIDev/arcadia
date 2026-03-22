# Arcadia.Notifications

Toast notification system for Blazor with auto-dismiss, stacking, and ARIA accessibility.

## Quick Start

```csharp
builder.Services.AddScoped<ToastService>();
```

```razor
<HelixToastContainer Position="ToastPosition.TopRight" />
```

```csharp
@inject ToastService ToastService

ToastService.ShowSuccess("File uploaded!");
ToastService.ShowError("Connection lost.");
```

## Installation

```
dotnet add package Arcadia.Notifications
```
