using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerBuilderTest
{
    public readonly List<RandomizerConfiguration> Configurations;

    public RandomizerBuilderTest()
    {
        Configurations = new List<RandomizerConfiguration>()
        {
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                AddToExcludeList = false,
                ClearZeros = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = true,
                AddToExcludeList = false,
                ClearZeros = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                AddToExcludeList = true,
                ClearZeros = false
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                AddToExcludeList = false,
                ClearZeros = true
            },
            new RandomizerConfiguration()
            {
                AllowNegativeValues = true,
                AddToExcludeList = true,
                ClearZeros = true
            }
        };
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void BuildWithDataRandomizer(int builderIndex)
    {
        var randomizer = new RandomizerBuilder<int>(Configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, 100 }
        }).Build();

        randomizer.GetRandom(10).Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void BuildWithExcludeRandomizer(int builderIndex)
    {
        var randomizer = new RandomizerBuilder<int>(Configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, 100 },
            { 2, 100 }
        }).Exclude(new List<int>()
        {
            2
        }).Build();

        randomizer.GetRandom(10).Should().NotBeEmpty().And.NotContain(2);
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
    public void BuildWithConfigurationRandomizer(int builderIndex, int count, int expectedCount)
    {
        var randomizer = new RandomizerBuilder<int>(Configurations[builderIndex]).Data(new Dictionary<int, int>()
        {
            { 1, -100 },
            { 2, 100 }
        }).Exclude(new List<int>()
        {
            2
        }).Build();
        
        randomizer.Add(1, 100);

        var values1 = randomizer.GetRandom(count);
        values1.Count.Should().Be(expectedCount);
    }
}