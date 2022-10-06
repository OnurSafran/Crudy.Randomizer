namespace Crudy.Randomizer;

public interface IReadonlyRandomizer<T> where T : notnull
{
    public IList<T> Draw(int count);
}