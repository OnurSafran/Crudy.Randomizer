using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crudy.Randomizer;

/// <summary>
/// Draw-only view of a randomizer: weights can't be changed through it, but a draw can still
/// consume or exclude keys when the configuration enables <see cref="RandomizerConfiguration.RemoveOnDraw"/>
/// or <see cref="RandomizerConfiguration.ExcludeOnDrawForUniqueness"/>.
/// </summary>
public interface IReadOnlyRandomizer<T> where T : notnull
{
    /// <summary>Draws up to <paramref name="count"/> keys, stopping early when nothing is left to draw.</summary>
    IList<T> Draw(int count);

    /// <summary>Draws one key, or <c>default</c> when nothing can be drawn. Prefer <see cref="TryDraw"/> for value types.</summary>
    T? Draw();

    /// <summary>Draws one key; returns <c>false</c> when nothing can be drawn.</summary>
    bool TryDraw([MaybeNullWhen(false)] out T key);
}
