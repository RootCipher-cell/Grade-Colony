public class Building
{
    public string Name { get; init; } = "";

    // Coins needed to build the first one
    public int Cost { get; init; }

    // Extra coins per turn for each one you own
    public int IncomeBonus { get; init; }

    // Extra coins per turn for each subject you entered, for each one you own
    public int IncomePerSubject { get; init; }

    // Extra % on top of your grade income, for each one you own
    public int GradeIncomePercent { get; init; }

    // % interest on your saved coins each turn, for each one you own
    public int InterestPercent { get; init; }

    // Resources needed to build the first one
    public Dictionary<ResourceType, int> ResourceCost { get; init; } = new Dictionary<ResourceType, int>();

    // Resources this building makes each turn
    public Dictionary<ResourceType, int> Produces { get; init; } = new Dictionary<ResourceType, int>();

    // Resources this building uses up each turn to work
    public Dictionary<ResourceType, int> Consumes { get; init; } = new Dictionary<ResourceType, int>();

    // Every building in the game. To add a new one, copy a block and change the numbers.
    public static readonly List<Building> All = new List<Building>
    {
        new Building
        {
            Name = "Farm",
            Cost = 100,
            Produces = new Dictionary<ResourceType, int> { [ResourceType.Food] = 5 }
        },
        new Building
        {
            Name = "Lumber Mill",
            Cost = 100,
            Produces = new Dictionary<ResourceType, int> { [ResourceType.Wood] = 5 }
        },
        new Building
        {
            Name = "Quarry",
            Cost = 150,
            ResourceCost = new Dictionary<ResourceType, int> { [ResourceType.Wood] = 20 },
            Produces = new Dictionary<ResourceType, int> { [ResourceType.Stone] = 5 }
        },
        new Building
        {
            Name = "Iron Mine",
            Cost = 200,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Wood] = 30,
                [ResourceType.Stone] = 20
            },
            Produces = new Dictionary<ResourceType, int> { [ResourceType.Iron] = 3 }
        },
        new Building
        {
            Name = "Barracks",
            Cost = 250,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Wood] = 30,
                [ResourceType.Stone] = 20,
                [ResourceType.Iron] = 10
            },
            Consumes = new Dictionary<ResourceType, int> { [ResourceType.Food] = 3 },
            Produces = new Dictionary<ResourceType, int> { [ResourceType.Soldiers] = 1 }
        },
        new Building
        {
            Name = "Library",
            Cost = 250,
            IncomeBonus = 30,
            ResourceCost = new Dictionary<ResourceType, int> { [ResourceType.Wood] = 20 }
        },
        new Building
        {
            Name = "Market",
            Cost = 500,
            IncomeBonus = 70,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Wood] = 40,
                [ResourceType.Stone] = 30
            }
        },
        new Building
        {
            Name = "Academy",
            Cost = 1000,
            IncomeBonus = 150,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Stone] = 60,
                [ResourceType.Iron] = 20
            }
        },
        new Building
        {
            Name = "Study Hall",
            Cost = 200,
            IncomePerSubject = 3,
            ResourceCost = new Dictionary<ResourceType, int> { [ResourceType.Wood] = 20 }
        },
        new Building
        {
            Name = "Bank",
            Cost = 400,
            InterestPercent = 2,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Wood] = 40,
                [ResourceType.Stone] = 30
            }
        },
        new Building
        {
            Name = "Tutoring Center",
            Cost = 600,
            GradeIncomePercent = 10,
            ResourceCost = new Dictionary<ResourceType, int>
            {
                [ResourceType.Stone] = 40,
                [ResourceType.Iron] = 20
            }
        }
    };
}