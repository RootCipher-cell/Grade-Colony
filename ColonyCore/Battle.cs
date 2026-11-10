public class BattleResult
{
    public string TargetName { get; init; } = "";
    public bool Won { get; init; }
    public int SoldiersSent { get; init; }
    public int AttackerLosses { get; init; }
    public int DefenderLosses { get; init; }
    public int LootCoins { get; init; }
    public Dictionary<ResourceType, int> LootResources { get; init; } = new Dictionary<ResourceType, int>();
}

public static class Battle
{
    // Share of the defender's coins and resources stolen when you win
    private const double LootFraction = 0.25;

    // Defenders fight slightly stronger at home
    private const double DefenderBonus = 1.15;

    private static readonly Random Rng = new Random();

    // Fights one battle and changes both the colony and the university.
    // Returns null (and changes nothing) if you try to send more soldiers than you have.
    public static BattleResult? Attack(Colony colony, University target, int sent)
    {
        int available = colony.Resources[ResourceType.Soldiers];
        if (sent <= 0 || sent > available) return null;

        int defenders = target.Resources[ResourceType.Soldiers];

        // Strength = soldiers x luck (0.8 to 1.2)
        double attackPower = sent * (0.8 + Rng.NextDouble() * 0.4);
        double defensePower = defenders * DefenderBonus * (0.8 + Rng.NextDouble() * 0.4);

        bool won = attackPower > defensePower;
        int attackerLosses;
        int defenderLosses;
        int lootCoins = 0;
        var loot = new Dictionary<ResourceType, int>();

        if (won)
        {
            // Winner loses fewer soldiers. Closer fights cost more.
            double ratio = defensePower / attackPower;   // below 1
            attackerLosses = (int)Math.Round(sent * ratio * 0.5);
            defenderLosses = (int)Math.Round(defenders * 0.7);

            lootCoins = (int)(target.Coins * LootFraction);
            target.Coins -= lootCoins;
            colony.AddCoins(lootCoins);

            // Soldiers are never stolen, only these four
            ResourceType[] stealable = { ResourceType.Food, ResourceType.Wood, ResourceType.Stone, ResourceType.Iron };
            foreach (ResourceType type in stealable)
            {
                int amount = (int)(target.Resources[type] * LootFraction);
                target.Resources[type] -= amount;
                colony.Resources[type] += amount;
                loot[type] = amount;
            }
        }
        else
        {
            double ratio = attackPower / defensePower;   // 1 or below
            attackerLosses = (int)Math.Round(sent * 0.7);
            defenderLosses = (int)Math.Round(defenders * ratio * 0.5);
        }

        // At least 1 soldier dies on each side if there was a real fight
        if (defenders > 0)
        {
            attackerLosses = Math.Max(1, attackerLosses);
            defenderLosses = Math.Max(1, defenderLosses);
        }

        attackerLosses = Math.Clamp(attackerLosses, 0, sent);
        defenderLosses = Math.Clamp(defenderLosses, 0, defenders);

        colony.Resources[ResourceType.Soldiers] -= attackerLosses;
        target.Resources[ResourceType.Soldiers] -= defenderLosses;

        return new BattleResult
        {
            TargetName = target.Name,
            Won = won,
            SoldiersSent = sent,
            AttackerLosses = attackerLosses,
            DefenderLosses = defenderLosses,
            LootCoins = lootCoins,
            LootResources = loot
        };
    }
}