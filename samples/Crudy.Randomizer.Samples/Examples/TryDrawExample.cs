namespace Crudy.Randomizer.Samples.Examples;

/// <summary>
/// Draw() returns default when nothing is left, which for an enum is its first member.
/// TryDraw tells "drew the first member" apart from "nothing left".
/// </summary>
public static class TryDrawExample
{
    private enum Rarity
    {
        Common,
        Rare,
        Epic
    }

    public static void Run()
    {
        var sack = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<Rarity>()
            .Data(new Dictionary<Rarity, int>()
            {
                { Rarity.Common, 2 },
                { Rarity.Epic, 1 }
            }).Build();

        while (sack.TryDraw(out var rarity))
            Console.WriteLine($"Opened a chest: {rarity}");

        Console.WriteLine($"No chests left. Draw() now returns {sack.Draw()}, which is why TryDraw exists.");
    }
}
