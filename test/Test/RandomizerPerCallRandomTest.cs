using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerPerCallRandomTest
{
    private static readonly Dictionary<int, int> Data = new()
    {
        { 1, 10 },
        { 2, 20 },
        { 3, 30 }
    };

    [Fact]
    public void PerCallRandomShouldMatchRandomGivenToBuilder()
    {
        var expected = new RandomizerBuilder<int>().Random(new Random(42)).Data(Data).Build().Draw(100);

        new RandomizerBuilder<int>().Data(Data).Build().Draw(100, new Random(42)).Should().Equal(expected);
    }

    [Fact]
    public void SameSeedPerCallShouldGiveSameKeysFromOneSharedRandomizer()
    {
        var randomizer = new RandomizerBuilder<int>().Data(Data).Build();

        for (var seed = 0; seed < 100; seed++)
        {
            randomizer.Draw(new Random(seed)).Should().Be(randomizer.Draw(new Random(seed)));
            randomizer.Draw(10, new Random(seed)).Should().Equal(randomizer.Draw(10, new Random(seed)));
        }
    }

    [Fact]
    public void PerCallRandomShouldNotAdvanceOwnRandomSource()
    {
        var expected = new RandomizerBuilder<int>().Random(new Random(7)).Data(Data).Build().Draw(50);
        var randomizer = new RandomizerBuilder<int>().Random(new Random(7)).Data(Data).Build();

        randomizer.Draw(20, new Random(1));
        randomizer.TryDraw(new Random(2), out _);
        randomizer.Draw(new Random(3));

        randomizer.Draw(50).Should().Equal(expected);
    }

    [Fact]
    public void PerCallRandomShouldReturnNothingWhenEmpty()
    {
        var randomizer = new Randomizer<int>();

        randomizer.TryDraw(new Random(1), out _).Should().BeFalse();
        randomizer.Draw(new Random(1)).Should().Be(default);
        randomizer.Draw(5, new Random(1)).Should().BeEmpty();
    }

    [Fact]
    public void PerCallRandomShouldRejectNull()
    {
        var randomizer = new RandomizerBuilder<int>().Data(Data).Build();

        randomizer.Invoking(r => r.Draw(null!)).Should().Throw<ArgumentNullException>();
        randomizer.Invoking(r => r.Draw(1, null!)).Should().Throw<ArgumentNullException>();
        randomizer.Invoking(r => r.TryDraw(null!, out _)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PerCallRandomShouldStillApplyConfiguration()
    {
        var sack = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<int>()
            .Data(new Dictionary<int, int>() { { 1, 2 }, { 2, 1 } })
            .Build();

        sack.Draw(10, new Random(1)).Should().HaveCount(3);
        sack.TryDraw(new Random(1), out _).Should().BeFalse();
    }

    [Fact]
    public void PoolShouldGiveSameKeysWhenSharedAcrossThreadsWithPerCallRandom()
    {
        var pool = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>().Data(Data).Build();
        var expected = Enumerable.Range(0, 1000).Select(seed => pool.Draw(20, new Random(seed))).ToList();
        var actual = new IList<int>[expected.Count];

        Parallel.For(0, expected.Count, seed => actual[seed] = pool.Draw(20, new Random(seed)));

        for (var seed = 0; seed < expected.Count; seed++)
            actual[seed].Should().Equal(expected[seed]);
    }
}
