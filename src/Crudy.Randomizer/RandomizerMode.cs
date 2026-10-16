namespace Crudy.Randomizer;

/// <summary>Preset configurations used by <see cref="RandomizerBuilderFactory"/>.</summary>
public enum RandomizerMode
{
    /// <summary>Weights are chances; drawing never changes them. Negative weights are allowed.</summary>
    Pool,

    /// <summary>Weights are amounts, like marbles in a sack; each draw takes one out. Negative weights become zero.</summary>
    Sack,

    /// <summary>The configuration given to the factory.</summary>
    Custom
}
