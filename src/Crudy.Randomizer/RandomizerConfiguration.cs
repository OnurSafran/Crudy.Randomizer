namespace Crudy.Randomizer;

/// <summary>
/// How a randomizer treats weights and draws. Copied when passed to a builder, factory or randomizer,
/// so later changes don't affect them.
/// </summary>
public class RandomizerConfiguration
{
    /// <summary>
    /// Keep weights below zero. When <c>false</c>, a weight that would go negative becomes <c>0</c>.
    /// Keys with a weight of zero or less are never drawn. Default <c>true</c>.
    /// </summary>
    public bool AllowNegativeValues { get; set; } = true;

    /// <summary>Remove a key as soon as its weight is <c>0</c>. When <c>false</c>, the key stays with weight <c>0</c>. Default <c>true</c>.</summary>
    public bool ClearZeros { get; set; } = true;

    /// <summary>Exclude a key after it's drawn, so each key is drawn at most once. Its weight is kept. Default <c>false</c>.</summary>
    public bool ExcludeOnDrawForUniqueness { get; set; } = false;

    /// <summary>Lower a key's weight by 1 each time it's drawn. Default <c>false</c>.</summary>
    public bool RemoveOnDraw { get; set; } = false;

    internal RandomizerConfiguration Copy() => (RandomizerConfiguration)MemberwiseClone();
}
