using System;
using System.Collections.Generic;

namespace Crudy.Randomizer;

/// <summary>A randomizer whose weights and excludes can be changed.</summary>
/// <typeparam name="T">Key type.</typeparam>
public interface IRandomizer<T> : IReadOnlyRandomizer<T> where T : notnull
{
    /// <summary>Adds each entry's weight; duplicate keys are summed.</summary>
    void Add(IEnumerable<KeyValuePair<T, int>> entries);

    /// <summary>
    /// Adds <paramref name="value"/> to the key's weight, creating the key if needed. A negative value lowers it.
    /// </summary>
    /// <exception cref="OverflowException">The weight would go beyond the range of <see cref="int"/>; nothing is changed.</exception>
    void Add(T key, int value);

    /// <summary>Removes the key and its weight. Does nothing if the key isn't there. Its exclude, if any, is kept.</summary>
    void Remove(T key);

    /// <summary>Removes each key. See <see cref="Remove(T)"/>.</summary>
    void Remove(IEnumerable<T> keys);

    /// <summary>Stops the key from being drawn while keeping its weight. The key doesn't need to exist yet.</summary>
    void AddExclude(T key);

    /// <summary>Excludes each key. See <see cref="AddExclude"/>.</summary>
    void AddExcludes(IEnumerable<T> keys);

    /// <summary>Lets an excluded key be drawn again.</summary>
    void RemoveExclude(T key);

    /// <summary>Lets each excluded key be drawn again.</summary>
    void RemoveExcludes(IEnumerable<T> keys);

    /// <summary>Lets every excluded key be drawn again.</summary>
    void RemoveAllExcludes();
}
