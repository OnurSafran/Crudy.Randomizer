using Crudy.Randomizer;
using FluentAssertions;

namespace Test;

public class RandomizerBuilderFactoryTest
{
    private static readonly Dictionary<int, int> Data = new()
    {
        { 1, 2 },
        { 2, 2 }
    };

    [Fact]
    public void PoolModeShouldNotConsumeOnDraw()
    {
        var randomizer = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>().Data(Data).Build();

        randomizer.Draw(100).Should().HaveCount(100);
    }

    [Fact]
    public void SackModeShouldConsumeOnDraw()
    {
        var randomizer = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<int>().Data(Data).Build();

        randomizer.Draw(100).Should().BeEquivalentTo(new[] { 1, 1, 2, 2 });
    }

    [Fact]
    public void CustomModeShouldUseGivenConfiguration()
    {
        var factory = new RandomizerBuilderFactory(new RandomizerConfiguration()
        {
            ExcludeOnDrawForUniqueness = true
        });

        factory.Mode.Should().Be(RandomizerMode.Custom);
        factory.GetBuilder<int>().Data(Data).Build().Draw(100).Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Fact]
    public void SetModeShouldChangeBuilderConfiguration()
    {
        var factory = new RandomizerBuilderFactory(RandomizerMode.Pool);
        factory.SetMode(RandomizerMode.Sack);

        factory.Mode.Should().Be(RandomizerMode.Sack);
        factory.GetBuilder<int>().Data(Data).Build().Draw(100).Should().HaveCount(4);
    }

    [Fact]
    public void GetBuilderWithModeShouldIgnoreCurrentMode()
    {
        var factory = new RandomizerBuilderFactory(RandomizerMode.Pool);

        factory.GetBuilder<int>(RandomizerMode.Sack).Data(Data).Build().Draw(100).Should().HaveCount(4);
        factory.Mode.Should().Be(RandomizerMode.Pool);
    }

    [Fact]
    public void ShouldNotBeAffectedByCustomConfigurationChangesAfterSet()
    {
        var configuration = new RandomizerConfiguration();
        var factory = new RandomizerBuilderFactory(configuration);

        configuration.RemoveOnDraw = true;

        factory.GetBuilder<int>().Data(Data).Build().Draw(100).Should().HaveCount(100);
    }
}
