namespace Crudy.Randomizer.Samples.Examples;

/// <summary>Weights are chances: drawing never changes them.</summary>
public static class LootTableExample
{
    public static void Run()
    {
        var lootTable = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<string>()
            .Data(new Dictionary<string, int>()
            {
                { "Gold", 60 },
                { "Potion", 30 },
                { "Rare Sword", 9 },
                { "Legendary Crown", 1 }
            }).Build();

        var drops = lootTable.Draw(10_000);

        foreach (var group in drops.GroupBy(drop => drop).OrderByDescending(group => group.Count()))
            Console.WriteLine($"{group.Key,-16} {group.Count(),5} drops  ({group.Count() / 100.0:F1}%)");
    }
}
