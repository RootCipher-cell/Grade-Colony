public class University
{
    public string Name { get; init; } = "";
    public int Coins { get; set; }

    // Same resource types as your colony (Food, Wood, Stone, Iron, Soldiers)
    public Dictionary<ResourceType, int> Resources { get; } = new Dictionary<ResourceType, int>();

    // Position on the map. A university with no coordinates is not shown on the map.
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public University(string name, int coins, int food, int wood, int stone, int iron, int soldiers)
    {
        Name = name;
        Coins = coins;

        foreach (ResourceType type in Enum.GetValues<ResourceType>())
        {
            Resources[type] = 0;
        }

        Resources[ResourceType.Food] = food;
        Resources[ResourceType.Wood] = wood;
        Resources[ResourceType.Stone] = stone;
        Resources[ResourceType.Iron] = iron;
        Resources[ResourceType.Soldiers] = soldiers;
    }

    // Every computer-controlled university. Edit freely:
    // name, coins, food, wood, stone, iron, soldiers
    //
    // To put a university on the map, add its coordinates after the closing bracket, like this:
    //   new University("Name", 400, 120, 90, 60, 30, 25) { Latitude = 36.8, Longitude = 10.1 },
    // (In the game, right-click a spot on the map and its coordinates are copied for you.)
    public static readonly List<University> All = new List<University>
    {
        // Approximate: a point on Boulevard du 9 Avril 1938, where the university is (92 Boulevard du 9 Avril 1938)
        new University("University of Tunis",                 400, 120,  90,  60, 30, 25) { Latitude = 36.7996, Longitude = 10.1637 },

        new University("Tunis El Manar University",           450, 130, 100,  70, 35, 30) { Latitude = 36.82922, Longitude = 10.14629 },

        // Around the university's headquarters in the Carthage / Amilcar area
        new University("University of Carthage",              500, 140, 110,  80, 40, 35) { Latitude = 36.87056, Longitude = 10.33444 },

        // Approximate: the center of the city of Manouba (the campus is in that city)
        new University("University of La Manouba",            350, 100,  80,  50, 25, 20) { Latitude = 36.80778, Longitude = 10.10111 },

        // Approximate: the center of the Montplaisir district (13 Rue Ibn Nadim, Montplaisir).
        // For the exact spot, right-click the building on the map and paste the coordinates here.
        new University("Virtual University of Tunis",         250,  80,  60,  40, 15, 10) { Latitude = 36.8158, Longitude = 10.1864 },

        new University("ESPRIT",                              600, 150, 120,  90, 50, 45) { Latitude = 36.899528, Longitude = 10.189556 },

        // Approximate: the source only gives the center of Tunis, to the nearest minute (03 Rue Hamadi Jaziri)
        new University("Centrale University",                 550, 140, 110,  85, 45, 40) { Latitude = 36.82, Longitude = 10.18 },

        // Approximate: on Avenue Khereddine Pacha, about 200 m southwest of the Tunisie Telecom office there (30 Avenue Khereddine Pacha).
        // For the exact spot, right-click the building on the map and paste the coordinates here.
        new University("Free University of Tunis",            300,  90,  70,  45, 20, 15) { Latitude = 36.8200, Longitude = 10.1876 },

        // The campus of South Mediterranean University in Berges du Lac, which MSB belongs to
        new University("Mediterranean School of Business",    380, 110,  85,  55, 28, 22) { Latitude = 36.845798, Longitude = 10.269064 },
    };

    // File format, one university per line: name;coins;Food=120,Wood=90,...
    public static void SaveAll(string path)
    {
        var lines = new List<string>();

        foreach (University u in All)
        {
            var parts = new List<string>();
            foreach (var pair in u.Resources)
            {
                parts.Add($"{pair.Key}={pair.Value}");
            }

            lines.Add($"{u.Name};{u.Coins};{string.Join(",", parts)}");
        }

        File.WriteAllLines(path, lines);
    }

    // Updates the universities in the list above with the saved numbers (matched by name)
    public static void LoadAll(string path)
    {
        if (!File.Exists(path)) return;

        foreach (string line in File.ReadAllLines(path))
        {
            string[] parts = line.Split(';');
            if (parts.Length < 3) continue;

            University? u = All.Find(x => x.Name == parts[0]);
            if (u == null || !int.TryParse(parts[1], out int coins)) continue;

            u.Coins = coins;

            foreach (string entry in parts[2].Split(','))
            {
                string[] pair = entry.Split('=');

                if (pair.Length == 2
                    && Enum.TryParse(pair[0], out ResourceType type)
                    && int.TryParse(pair[1], out int amount))
                {
                    u.Resources[type] = amount;
                }
            }
        }
    }
}