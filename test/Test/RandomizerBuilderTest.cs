using Crudy.Randomizer;

namespace Test;

public class UnitTest1
{
    [Fact]
    public void BuildRandomizer()
    {
        var builder = new RandomizerBuilder<int>();
    }
}