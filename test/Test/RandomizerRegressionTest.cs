using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerRegressionTest
{
    private enum Rarity
    {
        Common,
        Rare
    }

    [Fact]
    public void ShouldDrawKeyEqualToDefault()
    {
        var randomizer = new RandomizerBuilder<int>().Data(new Dictionary<int, int>()
        {
            { 0, 100 }
        }).Build();

        randomizer.Draw(10).Should().HaveCount(10).And.OnlyContain(x => x == 0);
        randomizer.TryDraw(out var value).Should().BeTrue();
        value.Should().Be(0);
    }

    [Fact]
    public void ShouldDrawFirstEnumMember()
    {
        var randomizer = new RandomizerBuilder<Rarity>().Data(new Dictionary<Rarity, int>()
        {
            { Rarity.Common, 100 },
            { Rarity.Rare, 0 }
        }).Build();

        randomizer.Draw(10).Should().HaveCount(10).And.OnlyContain(x => x == Rarity.Common);
    }

    [Fact]
    public void TryDrawShouldReturnFalseWhenEmpty()
    {
        var randomizer = new Randomizer<int>();

        randomizer.TryDraw(out _).Should().BeFalse();
        randomizer.Draw(10).Should().BeEmpty();
    }

    [Fact]
    public void SingleDrawShouldRemoveInSackMode()
    {
        var randomizer = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<string>()
            .Data(new Dictionary<string, int>()
            {
                { "a", 1 }
            }).Build();

        randomizer.Draw().Should().Be("a");
        randomizer.Draw().Should().BeNull();
        randomizer.TryDraw(out _).Should().BeFalse();
    }

    [Fact]
    public void SingleDrawShouldExcludeForUniqueness()
    {
        var randomizer = new RandomizerBuilder<string>(new RandomizerConfiguration()
        {
            ExcludeOnDrawForUniqueness = true
        }).Data(new Dictionary<string, int>()
        {
            { "a", 100 },
            { "b", 100 }
        }).Build();

        var first = randomizer.Draw();
        var second = randomizer.Draw();

        new[] { first, second }.Should().BeEquivalentTo(new[] { "a", "b" });
        randomizer.Draw().Should().BeNull();
    }

    [Fact]
    public void ReadmeCustomExampleShouldDrawBothKeys()
    {
        var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration()
        {
            AllowNegativeValues = false
        }).Data(new Dictionary<int, int>()
        {
            { 1, 100 },
            { 2, -100 }
        }).Build();
        randomizer.Add(2, 100);

        randomizer.Draw(1000).Should().Contain(1).And.Contain(2);
    }

    [Fact]
    public void ShouldNotBeAffectedByConfigurationChangesAfterBuild()
    {
        var configuration = new RandomizerConfiguration();
        var randomizer = new RandomizerBuilder<int>(configuration).Data(new Dictionary<int, int>()
        {
            { 1, 3 }
        }).Build();

        configuration.RemoveOnDraw = true;
        configuration.AllowNegativeValues = false;

        randomizer.Draw(10).Should().HaveCount(10);
    }

    [Fact]
    public void ShouldNotBeAffectedByFactoryCustomModeChanges()
    {
        var configuration = new RandomizerConfiguration();
        var randomizer = new RandomizerBuilderFactory(configuration).GetBuilder<int>()
            .Data(new Dictionary<int, int>()
            {
                { 1, 3 }
            }).Build();

        configuration.ExcludeOnDrawForUniqueness = true;

        randomizer.Draw(10).Should().HaveCount(10);
    }

    private static (Randomizer<int>, Randomizer<int>) CreateOverlappingRandomizers()
    {
        var randomizer1 = new RandomizerBuilder<int>().Data(new Dictionary<int, int>()
        {
            { 1, 10 },
            { 2, 10 }
        }).Build();
        var randomizer2 = new RandomizerBuilder<int>().Data(new Dictionary<int, int>()
        {
            { 2, 10 },
            { 3, 10 }
        }).Build();

        return (randomizer1, randomizer2);
    }

    [Fact]
    public void UnionShouldKeepAllKeys()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();

        randomizer1.Union(randomizer2).Print().Should().Be("1:10 , 2:20 , 3:10 , Total:40");
        (randomizer1 + randomizer2).Print().Should().Be("1:10 , 2:20 , 3:10 , Total:40");
    }

    [Fact]
    public void UpdateExistingShouldIgnoreNewKeys()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();

        randomizer1.UpdateExisting(randomizer2).Print().Should().Be("1:10 , 2:20 , Total:30");
        (randomizer1 * randomizer2).Print().Should().Be("1:10 , 2:20 , Total:30");
    }

    [Fact]
    public void IntersectShouldKeepOnlySharedKeys()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();

        var randomizer = randomizer1.Intersect(randomizer2);

        randomizer.Print().Should().Be("2:20 , Total:20");
        randomizer.Draw(50).Should().OnlyContain(x => x == 2);
    }

    [Fact]
    public void IntersectShouldKeepExcludesFromBothSides()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();
        randomizer2.AddExclude(2);

        var randomizer = randomizer1.Intersect(randomizer2);

        randomizer.Print().Should().Be("2:20 , Total:0");
        randomizer.Draw(10).Should().BeEmpty();
    }

    [Fact]
    public void CombiningShouldNotModifyOperands()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();

        _ = randomizer1.Union(randomizer2);
        _ = randomizer1.UpdateExisting(randomizer2);
        _ = randomizer1.Intersect(randomizer2);

        randomizer1.Print().Should().Be("1:10 , 2:10 , Total:20");
        randomizer2.Print().Should().Be("2:10 , 3:10 , Total:20");
    }

    [Fact]
    public void CloneShouldKeepZeroWeightKeys()
    {
        var randomizer = new Randomizer<int>(new RandomizerConfiguration()
        {
            ClearZeros = false
        });
        randomizer.Add(1, 5);
        randomizer.Add(1, -5);

        randomizer.Clone().Print().Should().Be(randomizer.Print());
    }

    [Fact]
    public void CloneShouldBeIndependent()
    {
        var randomizer = new Randomizer<int>();
        randomizer.Add(1, 100);
        randomizer.AddExclude(2);

        var clone = randomizer.Clone();
        clone.Add(2, 100);
        clone.RemoveExclude(2);
        clone.Add(1, 50);

        randomizer.Print().Should().Be("1:100 , Total:100");
        clone.Print().Should().Be("1:150 , 2:100 , Total:250");
    }

    [Fact]
    public void ShouldDrawWhenTotalExceedsIntRange()
    {
        var randomizer = new Randomizer<int>();
        randomizer.Add(1, int.MaxValue);
        randomizer.Add(2, 10);

        randomizer.Print().Should().EndWith($"Total:{(long)int.MaxValue + 10}");
        randomizer.Draw(100).Should().HaveCount(100);
    }

    [Fact]
    public void ShouldThrowOnWeightOverflowWithoutChangingState()
    {
        var randomizer = new Randomizer<int>();
        randomizer.Add(1, int.MaxValue);
        var before = randomizer.Print();

        var act = () => randomizer.Add(1, 1);

        act.Should().Throw<OverflowException>();
        randomizer.Print().Should().Be(before);
    }

    [Fact(Timeout = 5000)]
    public async Task DrawCountShouldStopWhenExhausted()
    {
        var randomizer = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<string>()
            .Data(new Dictionary<string, int>()
            {
                { "a", 1 }
            }).Build();

        var values = await Task.Run(() => randomizer.Draw(int.MaxValue));

        values.Should().Equal("a");
    }

    [Fact]
    public void IntersectOperatorShouldMatchIntersect()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();

        (randomizer1 & randomizer2).Print().Should().Be(randomizer1.Intersect(randomizer2).Print());
    }

    [Fact]
    public void IntersectShouldDropExcludesOfNonSharedKeys()
    {
        var (randomizer1, randomizer2) = CreateOverlappingRandomizers();
        randomizer1.AddExclude(1);

        var randomizer = randomizer1.Intersect(randomizer2);
        randomizer.Add(1, 50);

        randomizer.Print().Should().Be("2:20 , 1:50 , Total:70");
    }

    [Fact]
    public void ShouldKeepZeroWeightKeyWhenClearZerosDisabled()
    {
        var randomizer = new Randomizer<int>(new RandomizerConfiguration()
        {
            ClearZeros = false
        });
        randomizer.Add(7, 0);

        randomizer.Print().Should().Be("7:0 , Total:0");
    }

    [Fact]
    public void ShouldIgnoreZeroWeightKeyWhenClearZerosEnabled()
    {
        var randomizer = new Randomizer<int>();
        randomizer.Add(7, 0);

        randomizer.Print().Should().Be("Total:0");
    }

    [Fact]
    public void ShouldClampNewNegativeKeyToZeroWhenNegativesDisallowed()
    {
        var randomizer = new Randomizer<int>(new RandomizerConfiguration()
        {
            AllowNegativeValues = false,
            ClearZeros = false
        });
        randomizer.Add(1, -5);

        randomizer.Print().Should().Be("1:0 , Total:0");
    }

    [Fact]
    public void CombiningShouldKeepZeroWeightKeys()
    {
        var configuration = new RandomizerConfiguration()
        {
            ClearZeros = false
        };
        var randomizer1 = new Randomizer<int>(configuration);
        randomizer1.Add(1, 0);
        randomizer1.Add(2, 3);
        var randomizer2 = new Randomizer<int>(configuration);
        randomizer2.Add(1, 0);

        randomizer1.Union(new Randomizer<int>(configuration)).Print().Should().Be("1:0 , 2:3 , Total:3");
        randomizer1.UpdateExisting(randomizer2).Print().Should().Be("1:0 , 2:3 , Total:3");
        randomizer1.Intersect(randomizer2).Print().Should().Be("1:0 , Total:0");
    }

    [Fact]
    public void SeededRandomShouldBeReproducible()
    {
        Randomizer<int> Create() => new RandomizerBuilder<int>().Random(new Random(42)).Data(new Dictionary<int, int>()
        {
            { 1, 10 },
            { 2, 20 },
            { 3, 30 }
        }).Build();

        Create().Draw(100).Should().Equal(Create().Draw(100));
    }

    [Fact]
    public void DrawShouldFollowWeights()
    {
        const int draws = 100_000;
        var randomizer = new RandomizerBuilder<int>().Random(new Random(1)).Data(new Dictionary<int, int>()
        {
            { 1, 20 },
            { 2, 30 },
            { 3, 50 }
        }).Build();

        var counts = randomizer.Draw(draws).GroupBy(x => x).ToDictionary(g => g.Key, g => (double)g.Count() / draws);

        counts[1].Should().BeApproximately(0.2, 0.01);
        counts[2].Should().BeApproximately(0.3, 0.01);
        counts[3].Should().BeApproximately(0.5, 0.01);
    }

    [Fact]
    public void CloneAndCombiningShouldKeepRandomSource()
    {
        var data = new Dictionary<int, int>()
        {
            { 1, 10 },
            { 2, 20 }
        };
        var expected = new RandomizerBuilder<int>().Random(new Random(7)).Data(data).Build().Draw(50);

        new RandomizerBuilder<int>().Random(new Random(7)).Data(data).Build().Clone().Draw(50).Should().Equal(expected);
        (new RandomizerBuilder<int>().Random(new Random(7)).Data(data).Build() + new Randomizer<int>()).Draw(50)
            .Should().Equal(expected);
    }

    [Fact]
    public void ShouldUseComparer()
    {
        var randomizer = new RandomizerBuilder<string>().Comparer(StringComparer.OrdinalIgnoreCase).Data(new Dictionary<string, int>()
        {
            { "a", 1 }
        }).Build();

        randomizer.Add("A", 1);
        randomizer.Print().Should().Be("a:2 , Total:2");

        randomizer.AddExclude("A");
        randomizer.TryDraw(out _).Should().BeFalse();

        randomizer.Clone().Union(randomizer).Print().Should().Be("a:4 , Total:0");
    }

    [Fact]
    public void BuilderShouldAcceptAnyEnumerableAndSnapshotIt()
    {
        var data = new List<KeyValuePair<int, int>>()
        {
            new(1, 5),
            new(1, 5)
        };
        var builder = new RandomizerBuilder<int>().Data(data).Exclude(Enumerable.Empty<int>());
        data.Add(new(2, 100));

        builder.Build().Print().Should().Be("1:10 , Total:10");
    }

    [Fact]
    public void BuilderShouldNotBeAffectedByConfigurationChangesBeforeBuild()
    {
        var configuration = new RandomizerConfiguration();
        var builder = new RandomizerBuilder<int>(configuration).Data(new Dictionary<int, int>()
        {
            { 1, 3 }
        });

        configuration.RemoveOnDraw = true;

        builder.Build().Draw(10).Should().HaveCount(10);
    }

    [Fact]
    public void ShouldDrawManyKeysWithManyExcludesQuickly()
    {
        var randomizer = new Randomizer<int>();
        for (var i = 0; i < 20_000; i++) randomizer.Add(i, 1);
        randomizer.AddExcludes(Enumerable.Range(0, 10_000));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var values = randomizer.Draw(200);

        values.Should().HaveCount(200).And.OnlyContain(x => x >= 10_000);
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToStringShouldMatchPrint()
    {
        var (randomizer, _) = CreateOverlappingRandomizers();

        randomizer.ToString().Should().Be(randomizer.Print());
    }
}
