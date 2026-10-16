namespace Crudy.Randomizer.Samples.Examples;

/// <summary>Combining returns a new randomizer and leaves both inputs unchanged.</summary>
public static class CombiningExample
{
    public static void Run()
    {
        var baseLoot = new RandomizerBuilder<string>().Data(new Dictionary<string, int>()
        {
            { "Gold", 60 },
            { "Potion", 40 }
        }).Build();

        var halloweenEvent = new RandomizerBuilder<string>().Data(new Dictionary<string, int>()
        {
            { "Potion", 20 },
            { "Pumpkin", 30 }
        }).Build();

        Console.WriteLine($"base:          {baseLoot}");
        Console.WriteLine($"event:         {halloweenEvent}");
        Console.WriteLine($"base + event:  {baseLoot + halloweenEvent}   (all keys)");
        Console.WriteLine($"base * event:  {baseLoot * halloweenEvent}   (base keys only)");
        Console.WriteLine($"base & event:  {baseLoot & halloweenEvent}   (shared keys only)");
    }
}
