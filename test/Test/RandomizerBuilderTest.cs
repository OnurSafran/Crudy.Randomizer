using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerBuilderTest
{
    private readonly List<RandomizerConfiguration> _configurations;

    public RandomizerBuilderTest()
    {
        _configurations = new List<RandomizerConfiguration>()
        {
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                ExcludeOnDrawForUniqueness = false,
                ClearZeros = false,
                RemoveOnDraw = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = true,
                ExcludeOnDrawForUniqueness = false,
                ClearZeros = false,
                RemoveOnDraw = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                ExcludeOnDrawForUniqueness = true,
                ClearZeros = false,
                RemoveOnDraw = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                ExcludeOnDrawForUniqueness = false,
                ClearZeros = true,
                RemoveOnDraw = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = true,
                ExcludeOnDrawForUniqueness = true,
                ClearZeros = true,
                RemoveOnDraw = true
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                ExcludeOnDrawForUniqueness = false,
                ClearZeros = false,
                RemoveOnDraw = true
            }
        };
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void BuildWithDataRandomizer(int builderIndex)
    {
        var randomizer = new RandomizerBuilder<int>(_configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, 100 }
        }).Build();

        randomizer.Draw(10).Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void BuildWithExcludeRandomizer(int builderIndex)
    {
        var randomizer = new RandomizerBuilder<int>(_configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, 100 },
            { 2, 100 }
        }).Exclude(new List<int>()
        {
            2
        }).Build();

        randomizer.Draw(10).Should().NotBeEmpty().And.NotContain(2);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(0, 10, 10)]
    [InlineData(1, 1, 0)]
    [InlineData(1, 10, 0)]
    [InlineData(2, 1, 1)]
    [InlineData(2, 10, 1)]
    [InlineData(3, 1, 1)]
    [InlineData(3, 10, 10)]
    [InlineData(4, 1, 0)]
    [InlineData(4, 10, 0)]
    [InlineData(5, 1, 1)]
    [InlineData(5, 100, 100)]
    [InlineData(5, 110, 100)]
    public void BuildWithConfigurationRandomizer(int builderIndex, int count, int expectedCount)
    {
        var randomizer = new RandomizerBuilder<int>(_configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, -100 },
            { 2, 100 }
        }).Exclude(new List<int>()
        {
            2
        }).Build();
        
        randomizer.Add(1, 100);

        var values1 = randomizer.Draw(count);
        values1.Count.Should().Be(expectedCount);
    }
}