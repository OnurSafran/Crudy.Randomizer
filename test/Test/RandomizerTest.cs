using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerTest
{
    public readonly List<RandomizerConfiguration> Configurations;

    public RandomizerTest()
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
    public void ShouldGetRandomValuesRequestedAmount(int builderIndex, int count, int expectedCount)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, -100);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(count);

        values1.Count.Should().Be(expectedCount);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldRemove(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.Remove(1);
        randomizer.Add(2, 100);
        
        var values2 = randomizer.GetRandom(10);

        values1.Should().NotIntersectWith(values2);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldDecreaseToZero(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        randomizer.Add(4, -100);
        randomizer.Add(4, 100);
        randomizer.Add(4, -100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.Add(1, -100);
        randomizer.Add(2, 100);
        randomizer.Add(3, -100);
        
        var values2 = randomizer.GetRandom(10);

        values1.Should().NotIntersectWith(values2);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldAddAndRemoveExclude(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.Add(10, 100);
        randomizer.Add(11, 100);
        randomizer.Add(12, 100);
        randomizer.Add(5, 100);
        randomizer.Add(2, 100);
        randomizer.AddExclude(1);
        randomizer.AddExclude(5);
        randomizer.Add(6, 100);
        randomizer.AddExclude(6);
        randomizer.RemoveExclude(10);
        randomizer.RemoveExcludes(new List<int>()
        {
            11,
            22
        });
        
        var values2 = randomizer.GetRandom(10);

        values1.Should().NotIntersectWith(values2);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldRemoveAllExcludes(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.Add(2, 100);
        randomizer.AddExclude(1);
        randomizer.AddExclude(2);
        randomizer.AddExclude(3);
        randomizer.RemoveAllExcludes();
        
        var values2 = randomizer.GetRandom(10);

        values1.Count.Should().NotBe(0);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldRemoveWhenExcluded(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.AddExclude(1);
        randomizer.Remove(1);
        randomizer.RemoveExclude(1);
        randomizer.Add(2, 100);
        
        var values2 = randomizer.GetRandom(10);

        values1.Should().NotIntersectWith(values2);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldExcludeWhenRemoved(int builderIndex)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        
        var values1 = randomizer.GetRandom(10);

        randomizer.Add(5, 100);
        randomizer.Add(2, 100);
        randomizer.Remove(1);
        randomizer.AddExclude(1);
        randomizer.Add(1, 100);
        randomizer.AddExclude(5);
        randomizer.Add(6, 100);
        randomizer.AddExclude(6);
        
        var values2 = randomizer.GetRandom(10);

        values1.Should().NotIntersectWith(values2);
    }
    
    [Theory]
    [InlineData(0, new int[]{1})]
    [InlineData(1, new int[]{1,3})]
    [InlineData(2, new int[]{1})]
    [InlineData(3, new int[]{1})]
    [InlineData(4, new int[]{1,3})]
    public void ShouldCloneEverything(int builderIndex, int[] notExpectedList)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(1, 100);
        randomizer.Add(2, 100);
        randomizer.Add(3, -100);
        randomizer.AddExclude(1);

        var clonedRandomizer = randomizer.Clone();
        
        clonedRandomizer.Add(3, 100);
        var values2 = clonedRandomizer.GetRandom(50);

        values2.Should().NotIntersectWith(notExpectedList);
    }
    
    [Theory]
    [InlineData(0, new int[]{1})]
    [InlineData(1, new int[]{1,3})]
    [InlineData(2, new int[]{1})]
    [InlineData(3, new int[]{1})]
    [InlineData(4, new int[]{1,3})]
    public void ShouldAddAndRemove(int builderIndex, int[] notExpectedList)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        var randomizer2 = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(new Dictionary<int, int>()
        {
            {1, 100},
            {2, 100},
            {4, 100},
            {10, 100},
            {11, -100}
        });
        randomizer2.Add(new Dictionary<int, int>()
        {
            {5, 100},
            {6, 100},
            {10, -200},
            {11, 100}
        });
        randomizer.Add(3, -100);
        randomizer.Add(3, 100);
        randomizer.Add(9, 0);
        randomizer.AddExclude(1);
        randomizer.Remove(4);
        randomizer.Remove(9);
        randomizer.Remove(new List<int>()
        {
            5,
            6
        });
        randomizer.Add(randomizer2);
        randomizer.Add(10, 50);

        var values1 = randomizer.GetRandom(50);

        values1.Should().NotIntersectWith(notExpectedList);
    }
    
    [Theory]
    [InlineData(0, new int[]{1})]
    [InlineData(1, new int[]{1,3})]
    [InlineData(2, new int[]{1})]
    [InlineData(3, new int[]{1})]
    [InlineData(4, new int[]{1,3})]
    public void ShouldAddExistingKeys(int builderIndex, int[] notExpectedList)
    {
        var randomizer = new Randomizer<int>(Configurations[builderIndex]);
        randomizer.Add(new Dictionary<int, int>()
        {
            {1, 100},
            {2, 100},
            {4, 100}
        });
        randomizer.Add(3, -100);
        randomizer.Add(3, 100);
        randomizer.AddExclude(1);
        randomizer.Remove(4);

        var randomizer2 = new Randomizer<int>(Configurations[builderIndex]);
        randomizer2.Add(new Dictionary<int, int>()
        {
            {1, 100},
            {2, 100},
            {4, 100},
            {5, 100},
            {6, 100}
        });
        
        randomizer.AddExistingKeys(randomizer2);
        var values1 = randomizer.GetRandom(50);

        values1.Should().NotIntersectWith(notExpectedList);
    }
}