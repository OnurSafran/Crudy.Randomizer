namespace Crudy.Randomizer.Samples.Examples;

/// <summary>Weights are amounts: every draw takes one out until the sack is empty.</summary>
public static class MarbleSackExample
{
    public static void Run()
    {
        var sack = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<string>()
            .Data(new Dictionary<string, int>()
            {
                { "Red", 3 },
                { "Blue", 2 },
                { "Green", 1 }
            }).Build();

        Console.WriteLine($"Before: {sack}");
        Console.WriteLine($"Picked: {string.Join(", ", sack.Draw(2))}");
        Console.WriteLine($"Left:   {sack}");

        // Asking for more than is left only returns what remains
        Console.WriteLine($"Rest:   {string.Join(", ", sack.Draw(100))}");
        Console.WriteLine($"Empty:  {sack}");
    }
}
