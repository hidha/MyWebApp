# MyWebApp — ASP.NET Core MVC

A basic ASP.NET Core MVC web app with a textarea UI, ready for you to extend.

## Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Run locally

```bash
cd MyWebApp
dotnet run
```

Then open: http://localhost:5000

## Project structure

```
MyWebApp/
├── Controllers/
│   └── HomeController.cs     ← Add new actions here
├── Models/
│   └── TextInputModel.cs     ← Add/extend your models here
├── Views/
│   ├── Home/
│   │   └── Index.cshtml      ← Main page UI
│   └── Shared/
│       └── _Layout.cshtml    ← Site-wide layout
├── wwwroot/
│   └── css/site.css          ← Styles
├── Program.cs                ← App startup & services
└── MyWebApp.csproj
```

## Adding functionality

- **Process text** → edit the `[HttpPost] Index` action in `HomeController.cs`
- **Add a database** → install EF Core: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
- **New pages** → add a controller action + matching view in `Views/`
