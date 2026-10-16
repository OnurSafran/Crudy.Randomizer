namespace Crudy.Randomizer;

public interface IRandomizer<T> : IReadOnlyRandomizer<T> where T : notnull
{
    /// <summary>Adds each entry's weight; duplicate keys are summed.</summary>
    void Add(IEnumerable<KeyValuePair<T, int>> entries);
    void Add(T key, int value);
    void Remove(T key);
    void Remove(IEnumerable<T> keys);
    void AddExclude(T key);
    void AddExcludes(IEnumerable<T> keys);
    void RemoveExclude(T key);
    void RemoveExcludes(IEnumerable<T> keys);
    void RemoveAllExcludes();
}
