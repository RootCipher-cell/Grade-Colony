# Exercise Collection

This folder contains the supporting C# and .NET exercises from the programming practice collection, organized by purpose while preserving each project’s original source layout.

## Folders

- `Fundamentals/` — introductory console exercises covering input, arithmetic, conditions, loops, grades, and small calculators.
- `Applied/` — larger calculation and challenge exercises, including invoices, training costs, and shopping bills.
- `GUI-Experiments/` — Windows Forms experiments and earlier GUI prototypes.

## Running an exercise

From the repository root, restore and build the exercise solution:

```powershell
dotnet restore .\Exercises\Exercises.slnx
dotnet build .\Exercises\Exercises.slnx
```

To run one console project:

```powershell
dotnet run --project ".\Exercises\Fundamentals\Exercise1Welcome\Exercise1Welcome\Exercise1Welcome.csproj"
```

Windows Forms projects require Windows and the .NET 10 SDK. Build output, IDE metadata, and local save files are intentionally excluded from version control.
