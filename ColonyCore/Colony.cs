public class Colony
{
    // Each Bank can pay at most this much interest per turn
    public const int MaxInterestPerBank = 100;

    // How many of each building you own, by building name
    private readonly Dictionary<string, int> _owned = new Dictionary<string, int>();

    public int Coins { get; private set; }
    public int Turn { get; private set; }

    // Food, Wood, Stone, Iron and Soldiers you currently have
    public Dictionary<ResourceType, int> Resources { get; } = new Dictionary<ResourceType, int>();

    // Messages from the last turn (idle buildings, soldiers leaving...)
    public List<string> Log { get; } = new List<string>();

    public Colony()
    {
        Coins = 100;   // starting coins
        Turn = 1;

        foreach (ResourceType type in Enum.GetValues<ResourceType>())
        {
            Resources[type] = 0;
        }
    }

    // Used by battles to give you stolen coins
    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    public int GetOwned(Building building)
    {
        return _owned.TryGetValue(building.Name, out int count) ? count : 0;
    }

    // Each extra copy costs 50% of the base price more
    // Example: Farm costs 100, then 150, then 200...
    public int GetPrice(Building building)
    {
        return building.Cost + GetOwned(building) * building.Cost / 2;
    }

    // Same rule for the resource part of the cost
    public Dictionary<ResourceType, int> GetResourceCost(Building building)
    {
        var result = new Dictionary<ResourceType, int>();
        int owned = GetOwned(building);

        foreach (var pair in building.ResourceCost)
        {
            result[pair.Key] = pair.Value + owned * pair.Value / 2;
        }

        return result;
    }

    // Coins you earn from your grades: average x 5
    public int GradeIncome(double average)
    {
        return (int)Math.Round(average * 5);
    }

    // Coins you earn from your buildings (flat bonuses, subjects, tutoring, interest)
    public int BuildingIncome(double average, int subjectCount)
    {
        int gradeIncome = GradeIncome(average);
        int total = 0;

        foreach (Building b in Building.All)
        {
            int owned = GetOwned(b);
            if (owned == 0) continue;

            total += owned * b.IncomeBonus;
            total += owned * b.IncomePerSubject * subjectCount;
            total += gradeIncome * owned * b.GradeIncomePercent / 100;

            if (b.InterestPercent > 0)
            {
                int interestPerBank = Math.Min(Coins * b.InterestPercent / 100, MaxInterestPerBank);
                total += owned * interestPerBank;
            }
        }

        return total;
    }

    // Total coins per turn = grades + buildings
    public int CalculateIncome(double average, int subjectCount)
    {
        return GradeIncome(average) + BuildingIncome(average, subjectCount);
    }

    // True if you have at least these resources
    private bool Has(Dictionary<ResourceType, int> needed)
    {
        foreach (var pair in needed)
        {
            if (Resources[pair.Key] < pair.Value) return false;
        }

        return true;
    }

    public bool CanAfford(Building building)
    {
        return Coins >= GetPrice(building) && Has(GetResourceCost(building));
    }

    // Pays coins and resources, then adds one building.
    // Returns false (and spends nothing) if you can't afford it.
    public bool TryBuild(Building building)
    {
        if (!CanAfford(building)) return false;

        // Work out the cost BEFORE adding the building, because the price depends on how many you own
        int price = GetPrice(building);
        Dictionary<ResourceType, int> resourceCost = GetResourceCost(building);

        Coins -= price;
        foreach (var pair in resourceCost)
        {
            Resources[pair.Key] -= pair.Value;
        }

        _owned[building.Name] = GetOwned(building) + 1;
        return true;
    }

    // Selling gives back half of the coin price you paid for your most recent copy.
    // Resources are not refunded.
    public int GetRefund(Building building)
    {
        int owned = GetOwned(building);
        if (owned == 0) return 0;

        int lastPrice = building.Cost + (owned - 1) * building.Cost / 2;
        return lastPrice / 2;
    }

    // Removes one building and gives the refund. Returns false if you own none.
    public bool TrySell(Building building)
    {
        int owned = GetOwned(building);
        if (owned == 0) return false;

        // Work out the refund BEFORE removing the building
        int refund = GetRefund(building);

        Coins += refund;
        _owned[building.Name] = owned - 1;
        return true;
    }

    // Ends the turn: pays coin income, runs every building, feeds the soldiers.
    // Returns how many coins were earned.
    public int EndTurn(double average, int subjectCount)
    {
        Log.Clear();

        int income = CalculateIncome(average, subjectCount);
        Coins += income;

        // Buildings work in list order, so Farms feed the Barracks before the Barracks run
        foreach (Building b in Building.All)
        {
            int idle = 0;

            for (int i = 0; i < GetOwned(b); i++)
            {
                if (!Has(b.Consumes))
                {
                    idle++;
                    continue;
                }

                foreach (var pair in b.Consumes)
                {
                    Resources[pair.Key] -= pair.Value;
                }
                foreach (var pair in b.Produces)
                {
                    Resources[pair.Key] += pair.Value;
                }
            }

            if (idle > 0)
            {
                Log.Add($"{idle} {b.Name} idle: not enough resources to run.");
            }
        }

        // Every soldier eats 1 Food. If there isn't enough, the hungry ones leave.
        int soldiers = Resources[ResourceType.Soldiers];
        int food = Resources[ResourceType.Food];

        if (soldiers > food)
        {
            int deserters = soldiers - food;
            Resources[ResourceType.Soldiers] -= deserters;
            Resources[ResourceType.Food] = 0;
            Log.Add($"{deserters} soldier(s) left because there was not enough Food!");
        }
        else
        {
            Resources[ResourceType.Food] -= soldiers;
        }

        Turn++;
        return income;
    }

    // File format: coins;turn;Farm=2,Library=1;Food=5,Wood=3
    public void Save(string path)
    {
        var buildingParts = new List<string>();
        foreach (var pair in _owned)
        {
            buildingParts.Add($"{pair.Key}={pair.Value}");
        }

        var resourceParts = new List<string>();
        foreach (var pair in Resources)
        {
            resourceParts.Add($"{pair.Key}={pair.Value}");
        }

        File.WriteAllText(path,
            $"{Coins};{Turn};{string.Join(",", buildingParts)};{string.Join(",", resourceParts)}");
    }

    public static Colony Load(string path)
    {
        var colony = new Colony();
        if (!File.Exists(path)) return colony;

        string[] parts = File.ReadAllText(path).Trim().Split(';');

        if (parts.Length >= 2
            && int.TryParse(parts[0], out int coins)
            && int.TryParse(parts[1], out int turn))
        {
            colony.Coins = coins;
            colony.Turn = turn;
        }

        // Old save files have fewer parts, so only read each part if it exists
        if (parts.Length >= 3 && parts[2] != "")
        {
            foreach (string entry in parts[2].Split(','))
            {
                string[] pair = entry.Split('=');

                if (pair.Length == 2 && int.TryParse(pair[1], out int count))
                {
                    colony._owned[pair[0]] = count;
                }
            }
        }

        if (parts.Length >= 4 && parts[3] != "")
        {
            foreach (string entry in parts[3].Split(','))
            {
                string[] pair = entry.Split('=');

                if (pair.Length == 2
                    && Enum.TryParse(pair[0], out ResourceType type)
                    && int.TryParse(pair[1], out int amount))
                {
                    colony.Resources[type] = amount;
                }
            }
        }

        return colony;
    }
}