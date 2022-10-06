namespace Crudy.Randomizer;

public interface IRandomizer<T> : IReadonlyRandomizer<T> where T : notnull
{
    void Add(IDictionary<T, int> dictionary);
    void Add(T key, int value);
    void Remove(T key);
    void Remove(IList<T> keys);
    void AddExclude(T key);
    void AddExcludes(IList<T> keys);
    void RemoveExclude(T key);
    void RemoveExcludes(IList<T> keys);
    void RemoveAllExcludes();
}