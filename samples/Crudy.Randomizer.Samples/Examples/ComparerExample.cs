namespace Crudy.Randomizer.Samples.Examples;

/// <summary>A key comparer decides which keys are the same, e.g. ignoring case.</summary>
public static class ComparerExample
{
    public static void Run()
    {
        var votes = new RandomizerBuilder<string>()
            .Comparer(StringComparer.OrdinalIgnoreCase)
            .Build();

        foreach (var vote in new[] { "Pizza", "pizza", "PIZZA", "Sushi", "sushi", "Tacos" })
            votes.Add(vote, 1);

        Console.WriteLine($"Votes: {votes}");
        Console.WriteLine($"Tonight's dinner, picked by vote weight: {votes.Draw()}");
    }
}
