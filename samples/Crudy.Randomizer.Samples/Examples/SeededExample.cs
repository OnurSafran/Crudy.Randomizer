namespace Crudy.Randomizer.Samples.Examples;

/// <summary>A seeded Random gives the same draws every run: handy for tests, replays and daily challenges.</summary>
public static class SeededExample
{
    public static void Run()
    {
        var todaysSeed = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        IList<string> DailyQuests() => new RandomizerBuilder<string>()
            .Configuration(new RandomizerConfiguration() { ExcludeOnDrawForUniqueness = true })
            .Random(new Random(todaysSeed))
            .Data(new Dictionary<string, int>()
            {
                { "Defeat 10 slimes", 3 },
                { "Collect 5 herbs", 3 },
                { "Win a duel", 2 },
                { "Find a hidden chest", 1 }
            }).Build()
            .Draw(2);

        Console.WriteLine($"Player A's daily quests: {string.Join(", ", DailyQuests())}");
        Console.WriteLine($"Player B's daily quests: {string.Join(", ", DailyQuests())}  (same seed, same quests)");
    }
}
