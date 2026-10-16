namespace Crudy.Randomizer.Samples.Examples;

/// <summary>A sack where every card has an amount of 1 is a shuffled deck.</summary>
public static class DeckShuffleExample
{
    public static void Run()
    {
        var suits = new[] { "♠", "♥", "♦", "♣" };
        var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        var cards = suits.SelectMany(suit => ranks.Select(rank => rank + suit));

        var deck = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<string>()
            .Data(cards.Select(card => KeyValuePair.Create(card, 1)))
            .Build();

        Console.WriteLine($"Hand 1: {string.Join(" ", deck.Draw(5))}");
        Console.WriteLine($"Hand 2: {string.Join(" ", deck.Draw(5))}");
        Console.WriteLine($"Cards left in deck: {deck.Draw(int.MaxValue).Count}");
    }
}
