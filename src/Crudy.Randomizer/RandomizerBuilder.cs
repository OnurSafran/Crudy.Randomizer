namespace Crudy.Randomizer;

/// <summary>Builds a <see cref="Randomizer{T}"/>. Inputs are copied when passed in.</summary>
public class RandomizerBuilder<T> where T : notnull
{
    private RandomizerConfiguration? _randomizerConfiguration;
    private List<KeyValuePair<T, int>>? _entries;
    private List<T>? _excludedKeys;
    private Random? _random;
    private IEqualityComparer<T>? _comparer;

    public RandomizerBuilder()
    {
    }

    public RandomizerBuilder(RandomizerConfiguration randomizerConfiguration)
    {
        _randomizerConfiguration = randomizerConfiguration.Copy();
    }

    public RandomizerBuilder<T> Configuration(RandomizerConfiguration randomizerConfiguration)
    {
        _randomizerConfiguration = randomizerConfiguration.Copy();
        return this;
    }

    /// <summary>Sets the initial weights, replacing any set before. Duplicate keys are summed.</summary>
    public RandomizerBuilder<T> Data(IEnumerable<KeyValuePair<T, int>> entries)
    {
        _entries = entries.ToList();
        return this;
    }

    /// <summary>Sets the initially excluded keys, replacing any set before.</summary>
    public RandomizerBuilder<T> Exclude(IEnumerable<T> excludedKeys)
    {
        _excludedKeys = excludedKeys.ToList();
        return this;
    }

    /// <summary>Source of randomness; pass a seeded instance for reproducible draws.</summary>
    public RandomizerBuilder<T> Random(Random random)
    {
        _random = random;
        return this;
    }

    public RandomizerBuilder<T> Comparer(IEqualityComparer<T> comparer)
    {
        _comparer = comparer;
        return this;
    }

    public Randomizer<T> Build()
    {
        var randomizer = new Randomizer<T>(_randomizerConfiguration ?? new RandomizerConfiguration(), _random, _comparer);

        if (_entries != null)
            randomizer.Add(_entries);

        if (_excludedKeys != null)
            randomizer.AddExcludes(_excludedKeys);

        return randomizer;
    }
}
