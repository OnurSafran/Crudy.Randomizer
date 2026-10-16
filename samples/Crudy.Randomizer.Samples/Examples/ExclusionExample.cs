namespace Crudy.Randomizer.Samples.Examples;

/// <summary>Excluded keys keep their weight but can't be drawn until the exclude is removed.</summary>
public static class ExclusionExample
{
    public static void Run()
    {
        var enemies = new RandomizerBuilder<string>()
            .Data(new Dictionary<string, int>()
            {
                { "Slime", 50 },
                { "Goblin", 30 },
                { "Dragon", 5 }
            })
            .Exclude(new[] { "Dragon" }) // locked until the player levels up
            .Build();

        Console.WriteLine($"Level 1 spawns: {string.Join(", ", enemies.Draw(8))}");

        enemies.RemoveExclude("Dragon");
        enemies.Add("Dragon", 45); // and make it far more likely
        Console.WriteLine($"Level 9 spawns: {string.Join(", ", enemies.Draw(8))}");
    }
}
