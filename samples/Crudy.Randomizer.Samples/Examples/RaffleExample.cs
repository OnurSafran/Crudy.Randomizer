namespace Crudy.Randomizer.Samples.Examples;

/// <summary>
/// Weights are tickets bought; a person can only win once.
/// ExcludeOnDrawForUniqueness excludes each winner without touching their weight.
/// </summary>
public static class RaffleExample
{
    public static void Run()
    {
        var raffle = new RandomizerBuilder<string>(new RandomizerConfiguration()
        {
            ExcludeOnDrawForUniqueness = true
        })
            .Data(new Dictionary<string, int>()
            {
                { "Alice", 5 },
                { "Bob", 1 },
                { "Carol", 3 },
                { "Dave", 1 }
            }).Build();

        var winners = raffle.Draw(3);
        for (var i = 0; i < winners.Count; i++)
            Console.WriteLine($"Prize {i + 1}: {winners[i]}");

        // Reset for the next raffle with the same tickets
        raffle.RemoveAllExcludes();
        Console.WriteLine($"Next raffle's first prize: {raffle.Draw()}");
    }
}
