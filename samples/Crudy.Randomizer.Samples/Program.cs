using Crudy.Randomizer.Samples.Examples;

// Run all examples:            dotnet run --project samples/Crudy.Randomizer.Samples
// Run one example by name:     dotnet run --project samples/Crudy.Randomizer.Samples -- loot
var examples = new Dictionary<string, (string Title, Action Run)>(StringComparer.OrdinalIgnoreCase)
{
    ["loot"] = ("Loot table (Pool mode)", LootTableExample.Run),
    ["sack"] = ("Marbles in a sack (Sack mode)", MarbleSackExample.Run),
    ["deck"] = ("Shuffling a deck of cards", DeckShuffleExample.Run),
    ["raffle"] = ("Raffle with unique winners", RaffleExample.Run),
    ["exclude"] = ("Temporarily excluding keys", ExclusionExample.Run),
    ["combine"] = ("Combining randomizers", CombiningExample.Run),
    ["seed"] = ("Reproducible draws with a seed", SeededExample.Run),
    ["comparer"] = ("Case-insensitive keys", ComparerExample.Run),
    ["trydraw"] = ("TryDraw with value-type keys", TryDrawExample.Run),
};

var selected = args.Length == 0 ? examples.Keys.ToList() : args.ToList();

foreach (var name in selected)
{
    if (!examples.TryGetValue(name, out var example))
    {
        Console.WriteLine($"Unknown example '{name}'. Available: {string.Join(", ", examples.Keys)}");
        return 1;
    }

    Console.WriteLine($"=== {example.Title} ===");
    example.Run();
    Console.WriteLine();
}

return 0;
