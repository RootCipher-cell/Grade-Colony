# Grade Colony

Grade Colony is a .NET game and grade-management project that turns academic progress into a small colony-building strategy experience. Enter subjects and grades, earn income from your average, expand a colony with buildings and resources, and challenge rival universities. The project includes a reusable domain library, a console application, and a Windows Forms interface with a university map and image assets.

## Features

- Add, edit, remove, and review subjects with weighted grades.
- Calculate averages and academic mentions from the grade tracker.
- Build and sell colony buildings using coins and resources.
- Advance turns to collect grade- and building-based income.
- Manage Food, Wood, Stone, Iron, and Soldiers.
- Attack rival universities and resolve battles with losses and loot.
- Save and restore local progress through simple text save files.
- View the colony through a Windows Forms GUI with tabs for grades, buildings, colony status, and a map.
- Display university image assets in the GUI.

## Architecture

```text
Grade-Colony.slnx
├── ColonyCore/
│   └── Shared domain models and game rules
├── ColonyGrade Project/
│   └── Console application using ColonyCore
└── GUICOLONYPROJECT/
    └── Windows Forms application using ColonyCore
```

`ColonyCore` is the single source of truth for grades, colonies, buildings, resources, universities, and battles. Both front ends reference it directly, keeping game behavior consistent between the console and GUI applications.

## Requirements

- Windows 10 or later for the Windows Forms application.
- .NET 10 SDK.
- Internet access on the first GUI build so NuGet can restore `Ibdg.GMap.NET.WinForms`.

## Build

From the repository root:

```powershell
dotnet restore .\Grade-Colony.slnx
dotnet build .\Grade-Colony.slnx
```

The solution builds the shared library, console app, and Windows Forms GUI. Build output is intentionally excluded from version control.

## Run

### Console application

```powershell
dotnet run --project ".\ColonyGrade Project\ColonyGrade Project.csproj"
```

### Windows Forms GUI

```powershell
dotnet run --project ".\GUICOLONYPROJECT\GUICOLONYPROJECT.csproj"
```

The applications create `grades.txt`, `colony.txt`, and `universities.txt` beside the running application when progress is saved. These files are local state and are ignored by Git.

## Technologies

- C# and .NET 10
- .NET class library project references
- Windows Forms
- `Ibdg.GMap.NET.WinForms` for the map view
- Text-based local persistence

## Future improvements

- Add automated unit tests for grade calculations, resource production, and battle outcomes.
- Move save data to a user-specific application-data directory instead of the working directory.
- Add explicit save-file versioning and stronger validation for corrupted data.
- Improve accessibility, responsive layout, and localization in the GUI.
- Add richer progression, quests, balancing tools, and configurable university scenarios.
- Add CI validation for restore, build, and tests on every pull request.
