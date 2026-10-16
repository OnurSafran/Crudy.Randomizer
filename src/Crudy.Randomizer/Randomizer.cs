using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Crudy.Randomizer;

/// <summary>
/// Weighted randomizer. Not thread-safe: use one instance per thread or synchronize access.
/// </summary>
public class Randomizer<T> : IRandomizer<T> where T : notnull
{
    private readonly Dictionary<T, int> _dictionary;
    private readonly HashSet<T> _excludedKeys;
    private readonly RandomizerConfiguration _randomizerConfiguration;
    private readonly Random _random;
    private long _total;

    public Randomizer() : this(new RandomizerConfiguration())
    {
    }

    /// <param name="randomizerConfiguration">Copied, so later changes to it don't alter this randomizer.</param>
    /// <param name="random">Source of randomness; pass a seeded instance for reproducible draws. Defaults to <see cref="Random.Shared"/>.</param>
    /// <param name="comparer">Key comparer. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
    public Randomizer(RandomizerConfiguration randomizerConfiguration, Random? random = null, IEqualityComparer<T>? comparer = null)
    {
        _dictionary = new Dictionary<T, int>(comparer);
        _excludedKeys = new HashSet<T>(comparer);
        _randomizerConfiguration = randomizerConfiguration.Copy();
        _random = random ?? Random.Shared;
        _total = 0;
    }

    public void Add(IEnumerable<KeyValuePair<T, int>> entries)
    {
        foreach (var entry in entries) Add(entry.Key, entry.Value);
    }

    public void Add(T key, int value)
    {
        var exists = _dictionary.TryGetValue(key, out var oldValue);
        if (exists && value == 0)
            return;

        // Throws OverflowException before any state is touched
        var newValue = checked(oldValue + value);

        if (newValue < 0 && !_randomizerConfiguration.AllowNegativeValues)
            newValue = 0;

        if (newValue == 0 && _randomizerConfiguration.ClearZeros)
        {
            if (exists)
                Remove(key);
            return;
        }

        if (!_excludedKeys.Contains(key))
            _total += Math.Max(newValue, 0) - Math.Max(oldValue, 0);

        _dictionary[key] = newValue;
    }

    public void Remove(T key)
    {
        if (!_dictionary.Remove(key, out var value))
            return;

        if (value > 0 && !_excludedKeys.Contains(key))
            _total -= value;
    }

    public void Remove(IEnumerable<T> keys)
    {
        foreach (var key in keys.ToList()) Remove(key);
    }

    public void AddExclude(T key)
    {
        if (!_excludedKeys.Add(key))
            return;

        if (_dictionary.TryGetValue(key, out var value) && value > 0)
            _total -= value;
    }

    public void AddExcludes(IEnumerable<T> keys)
    {
        foreach (var key in keys.ToList()) AddExclude(key);
    }

    public void RemoveExclude(T key)
    {
        if (!_excludedKeys.Remove(key))
            return;

        if (_dictionary.TryGetValue(key, out var value) && value > 0)
            _total += value;
    }

    public void RemoveExcludes(IEnumerable<T> keys)
    {
        foreach (var key in keys.ToList()) RemoveExclude(key);
    }

    public void RemoveAllExcludes()
    {
        RemoveExcludes(_excludedKeys);
    }

    public string Print()
    {
        var stringBuilder = new StringBuilder();

        foreach (var entry in _dictionary)
        {
            stringBuilder.Append($"{entry.Key}:{entry.Value}");
            stringBuilder.Append(" , ");
        }

        stringBuilder.Append($"Total:{_total}");

        return stringBuilder.ToString();
    }

    public override string ToString() => Print();

    public T? Draw()
    {
        return TryDraw(out var key) ? key : default;
    }

    public bool TryDraw([MaybeNullWhen(false)] out T key)
    {
        if (!TryPick(out key))
            return false;

        if (_randomizerConfiguration.ExcludeOnDrawForUniqueness)
            AddExclude(key);

        if (_randomizerConfiguration.RemoveOnDraw)
            Add(key, -1);

        return true;
    }

    public IList<T> Draw(int count)
    {
        var list = new List<T>();

        for (var i = 0; i < count; i++)
        {
            if (!TryDraw(out var key))
                break;

            list.Add(key);
        }

        return list;
    }

    private bool TryPick([MaybeNullWhen(false)] out T key)
    {
        if (_total <= 0)
        {
            key = default;
            return false;
        }

        long counter = 0, rand = _random.NextInt64(0, _total);

        foreach (var entry in _dictionary)
        {
            if (entry.Value <= 0 || _excludedKeys.Contains(entry.Key))
                continue;

            counter += entry.Value;

            if (counter > rand)
            {
                key = entry.Key;
                return true;
            }
        }

        key = default;
        return false;
    }

    /// <summary>Independent copy sharing the configuration, random source and comparer.</summary>
    public Randomizer<T> Clone()
    {
        var randomizer = CreateEmpty();

        foreach (var entry in _dictionary) randomizer._dictionary.Add(entry.Key, entry.Value);
        randomizer._excludedKeys.UnionWith(_excludedKeys);
        randomizer._total = _total;

        return randomizer;
    }

    private Randomizer<T> CreateEmpty()
    {
        return new Randomizer<T>(_randomizerConfiguration, _random, _dictionary.Comparer);
    }

    private void Add(Randomizer<T> randomizer)
    {
        foreach (var entry in randomizer._dictionary) Add(entry.Key, entry.Value);
    }

    private void AddExistingKeys(Randomizer<T> randomizer)
    {
        foreach (var entry in randomizer._dictionary.Where(entry => _dictionary.ContainsKey(entry.Key)))
            Add(entry.Key, entry.Value);
    }

    // The combining methods below return a new randomizer using this randomizer's
    // configuration, random source and comparer; both inputs are left unchanged.

    /// <summary>All keys from both randomizers, weights summed. Same as <c>+</c>.</summary>
    public Randomizer<T> Union(Randomizer<T> other)
    {
        var randomizer = CreateEmpty();

        randomizer.Add(this);
        randomizer.Add(other);

        randomizer.AddExcludes(_excludedKeys);
        randomizer.AddExcludes(other._excludedKeys);

        return randomizer;
    }

    /// <summary>Keys of this randomizer, with weights from <paramref name="other"/> added to the shared keys. Same as <c>*</c>.</summary>
    public Randomizer<T> UpdateExisting(Randomizer<T> other)
    {
        var randomizer = CreateEmpty();

        randomizer.Add(this);
        randomizer.AddExistingKeys(other);

        randomizer.AddExcludes(_excludedKeys);

        return randomizer;
    }

    /// <summary>
    /// Only keys present in both randomizers, weights summed. Excludes from either side are kept
    /// for those keys only. Same as <c>&amp;</c>.
    /// </summary>
    public Randomizer<T> Intersect(Randomizer<T> other)
    {
        var randomizer = CreateEmpty();
        var sharedKeys = _dictionary.Keys.Where(other._dictionary.ContainsKey).ToList();

        foreach (var key in sharedKeys)
        {
            randomizer.Add(key, _dictionary[key]);
            randomizer.Add(key, other._dictionary[key]);
        }

        randomizer.AddExcludes(sharedKeys.Where(key => _excludedKeys.Contains(key) || other._excludedKeys.Contains(key)));

        return randomizer;
    }

    public static Randomizer<T> operator +(Randomizer<T> r1, Randomizer<T> r2) => r1.Union(r2);

    public static Randomizer<T> operator *(Randomizer<T> r1, Randomizer<T> r2) => r1.UpdateExisting(r2);

    public static Randomizer<T> operator &(Randomizer<T> r1, Randomizer<T> r2) => r1.Intersect(r2);
}
