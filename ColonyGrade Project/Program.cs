using System.Globalization;

const string GradesPath = "grades.txt";
const string ColonyPath = "colony.txt";
const string UniversitiesPath = "universities.txt";

var tracker = new GradeTracker();
tracker.Load(GradesPath);
Colony colony = Colony.Load(ColonyPath);
University.LoadAll(UniversitiesPath);

bool running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== Grade Colony ===");
    Console.WriteLine("1. Add a subject and grade");
    Console.WriteLine("2. View all grades and average");
    Console.WriteLine("3. Edit a grade");
    Console.WriteLine("4. Remove a subject");
    Console.WriteLine("5. View colony");
    Console.WriteLine("6. Build");
    Console.WriteLine("7. Sell a building");
    Console.WriteLine("8. End turn (collect coins)");
    Console.WriteLine("9. Attack a university");
    Console.WriteLine("10. Save and exit");
    Console.Write("Choose: ");

    switch (Console.ReadLine())
    {
        case "1":
            AddSubject(tracker);
            break;
        case "2":
            ShowAll(tracker);
            break;
        case "3":
            EditGrade(tracker);
            break;
        case "4":
            RemoveSubject(tracker);
            break;
        case "5":
            ShowColony(tracker, colony);
            break;
        case "6":
            BuildMenu(colony);
            break;
        case "7":
            SellMenu(colony);
            break;
        case "8":
            EndTurn(tracker, colony);
            break;
        case "9":
            AttackMenu(colony);
            break;
        case "10":
            tracker.Save(GradesPath);
            colony.Save(ColonyPath);
            University.SaveAll(UniversitiesPath);
            Console.WriteLine("Saved. Goodbye!");
            running = false;
            break;
        default:
            Console.WriteLine("Please choose 1-10.");
            break;
    }
}

// ---------- helper methods ----------

void AddSubject(GradeTracker t)
{
    Console.Write("Subject name: ");
    string name = (Console.ReadLine() ?? "").Trim().Replace(';', ' ');
    if (name == "")
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    double grade = ReadNumber("Grade (0-20): ", 0, 20);
    double coefficient = ReadNumber("Coefficient (use 1 if all subjects count the same): ", 0.1, 10);

    t.Add(new Subject(name, grade, coefficient));
    Console.WriteLine($"Added. Result: {Subject.GetMention(grade)}");
}

void ShowAll(GradeTracker t)
{
    if (t.Subjects.Count == 0)
    {
        Console.WriteLine("No subjects yet.");
        return;
    }

    for (int i = 0; i < t.Subjects.Count; i++)
    {
        Subject s = t.Subjects[i];
        Console.WriteLine($"{i + 1}. {s.Name}: {s.Grade:0.##} (coef {s.Coefficient:0.##}) - {s.Mention}");
    }

    double average = t.CalculateAverage();
    Console.WriteLine($"Average: {average:0.00} - {Subject.GetMention(average)}");
}

void EditGrade(GradeTracker t)
{
    if (t.Subjects.Count == 0)
    {
        Console.WriteLine("No subjects yet.");
        return;
    }

    ShowAll(t);
    int number = (int)ReadNumber("Which subject number to edit? ", 1, t.Subjects.Count);
    t.Subjects[number - 1].Grade = ReadNumber("New grade (0-20): ", 0, 20);
    Console.WriteLine("Updated.");
}

void RemoveSubject(GradeTracker t)
{
    if (t.Subjects.Count == 0)
    {
        Console.WriteLine("No subjects yet.");
        return;
    }

    ShowAll(t);
    int number = (int)ReadNumber("Which subject number to remove? ", 1, t.Subjects.Count);
    t.Remove(number - 1);
    Console.WriteLine("Removed.");
}

void ShowColony(GradeTracker t, Colony c)
{
    double average = t.CalculateAverage();

    Console.WriteLine("=== Your colony ===");
    Console.WriteLine($"Turn: {c.Turn}");
    Console.WriteLine($"Coins: {c.Coins}");
    Console.WriteLine($"Resources: {FormatResources(c.Resources)}");
    int totalIncome = c.CalculateIncome(average, t.Subjects.Count);
    int fromGrades = c.GradeIncome(average);
    Console.WriteLine($"Income per turn: {totalIncome} (grades {fromGrades} + buildings {totalIncome - fromGrades})");

    Console.WriteLine("Buildings:");
    bool hasAny = false;
    foreach (Building b in Building.All)
    {
        int owned = c.GetOwned(b);
        if (owned > 0)
        {
            Console.WriteLine($"  {b.Name}: {owned}");
            hasAny = true;
        }
    }
    if (!hasAny)
    {
        Console.WriteLine("  None yet. Choose 6 to build.");
    }
}

void BuildMenu(Colony c)
{
    Console.WriteLine($"=== Build (you have {c.Coins} coins) ===");
    Console.WriteLine($"Resources: {FormatResources(c.Resources)}");

    for (int i = 0; i < Building.All.Count; i++)
    {
        Building b = Building.All[i];
        Console.WriteLine($"{i + 1}. {b.Name} (you own {c.GetOwned(b)})");
        Console.WriteLine($"     Cost: {CostText(c, b)}");
        Console.WriteLine($"     Effect: {EffectText(b)}");
    }
    Console.WriteLine("0. Back");

    int choice = (int)ReadNumber("Choose: ", 0, Building.All.Count);
    if (choice == 0) return;

    Building chosen = Building.All[choice - 1];

    if (c.TryBuild(chosen))
    {
        Console.WriteLine($"Built a {chosen.Name}! Coins left: {c.Coins}");
    }
    else
    {
        Console.WriteLine("Not enough coins or resources.");
    }
}

// Turns a set of resources into text, like "5 Food, 3 Wood"
string FormatResources(Dictionary<ResourceType, int> amounts)
{
    var parts = new List<string>();

    foreach (var pair in amounts)
    {
        parts.Add($"{pair.Value} {pair.Key}");
    }

    return string.Join(", ", parts);
}

string CostText(Colony c, Building b)
{
    string text = $"{c.GetPrice(b)} coins";
    Dictionary<ResourceType, int> resourceCost = c.GetResourceCost(b);

    if (resourceCost.Count > 0)
    {
        text += ", " + FormatResources(resourceCost);
    }

    return text;
}

string EffectText(Building b)
{
    var effects = new List<string>();

    if (b.IncomeBonus > 0) effects.Add($"+{b.IncomeBonus} coins per turn");
    if (b.IncomePerSubject > 0) effects.Add($"+{b.IncomePerSubject} coins per subject you entered");
    if (b.GradeIncomePercent > 0) effects.Add($"+{b.GradeIncomePercent}% of your grade income");
    if (b.InterestPercent > 0) effects.Add($"{b.InterestPercent}% interest on saved coins (max {Colony.MaxInterestPerBank} per turn)");
    if (b.Produces.Count > 0) effects.Add($"makes {FormatResources(b.Produces)} per turn");
    if (b.Consumes.Count > 0) effects.Add($"uses {FormatResources(b.Consumes)} per turn");

    return string.Join("; ", effects);
}

void SellMenu(Colony c)
{
    var owned = new List<Building>();

    foreach (Building b in Building.All)
    {
        if (c.GetOwned(b) > 0)
        {
            owned.Add(b);
        }
    }

    if (owned.Count == 0)
    {
        Console.WriteLine("You have no buildings to sell.");
        return;
    }

    Console.WriteLine("=== Sell a building (you get half the coin price back) ===");

    for (int i = 0; i < owned.Count; i++)
    {
        Building b = owned[i];
        Console.WriteLine($"{i + 1}. {b.Name} (you own {c.GetOwned(b)}) - refund {c.GetRefund(b)} coins");
    }
    Console.WriteLine("0. Back");

    int choice = (int)ReadNumber("Choose: ", 0, owned.Count);
    if (choice == 0) return;

    Building chosen = owned[choice - 1];
    int refund = c.GetRefund(chosen);

    c.TrySell(chosen);
    Console.WriteLine($"Sold a {chosen.Name} for {refund} coins. Coins: {c.Coins}");
}

void EndTurn(GradeTracker t, Colony c)
{
    int income = c.EndTurn(t.CalculateAverage(), t.Subjects.Count);
    Console.WriteLine($"Turn {c.Turn - 1} finished. You earned {income} coins. Total: {c.Coins}");

    foreach (string line in c.Log)
    {
        Console.WriteLine($"  {line}");
    }
    Console.WriteLine($"Resources: {FormatResources(c.Resources)}");

    if (income == 0)
    {
        Console.WriteLine("Add some grades to earn more coins.");
    }
}

void AttackMenu(Colony c)
{
    int soldiers = c.Resources[ResourceType.Soldiers];
    if (soldiers <= 0)
    {
        Console.WriteLine("You have no soldiers. Build Barracks first.");
        return;
    }

    Console.WriteLine("=== Universities of Tunis ===");
    for (int i = 0; i < University.All.Count; i++)
    {
        University u = University.All[i];
        Console.WriteLine($"{i + 1}. {u.Name} (soldiers: {u.Resources[ResourceType.Soldiers]})");
    }
    Console.WriteLine("0. Back");

    int choice = (int)ReadNumber("Choose a target: ", 0, University.All.Count);
    if (choice == 0) return;

    University target = University.All[choice - 1];
    int sent = (int)ReadNumber($"How many soldiers to send? (you have {soldiers}): ", 1, soldiers);

    BattleResult? result = Battle.Attack(c, target, sent);
    if (result == null) return;

    Console.WriteLine();
    Console.WriteLine(result.Won
        ? $"VICTORY against {result.TargetName}!"
        : $"DEFEAT. {result.TargetName} held their ground.");
    Console.WriteLine($"You lost {result.AttackerLosses} soldiers. They lost {result.DefenderLosses}.");

    if (result.Won)
    {
        Console.WriteLine($"Loot: {result.LootCoins} coins, {FormatResources(result.LootResources)}");
    }
}

// Keeps asking until the user types a valid number in range (accepts 12.5 or 12,5)
double ReadNumber(string prompt, double min, double max)
{
    while (true)
    {
        Console.Write(prompt);
        string input = (Console.ReadLine() ?? "").Replace(',', '.');

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            && value >= min && value <= max)
        {
            return value;
        }

        Console.WriteLine($"Please enter a number between {min} and {max}.");
    }
}