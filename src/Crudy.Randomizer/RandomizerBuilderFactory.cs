namespace Crudy.Randomizer;

/// <summary>
/// Hands out builders preconfigured for a <see cref="RandomizerMode"/>. Safe to share once configured;
/// <see cref="SetMode"/> and <see cref="SetCustomMode"/> are not thread-safe, so call them at startup only.
/// </summary>
public class RandomizerBuilderFactory
{
    private readonly Dictionary<RandomizerMode, RandomizerConfiguration> _configurationMap = new()
    {
        {
            RandomizerMode.Pool, new RandomizerConfiguration()
            {
                AllowNegativeValues = true,
                ClearZeros = true,
                ExcludeOnDrawForUniqueness = false,
                RemoveOnDraw = false
            }
        },
        {
            RandomizerMode.Sack, new RandomizerConfiguration()
            {
                AllowNegativeValues = false,
                ClearZeros = true,
                ExcludeOnDrawForUniqueness = false,
                RemoveOnDraw = true
            }
        },
        { RandomizerMode.Custom, new RandomizerConfiguration() }
    };

    public RandomizerBuilderFactory(RandomizerConfiguration customModeConfiguration)
    {
        SetCustomMode(customModeConfiguration);

        Mode = RandomizerMode.Custom;
    }

    public RandomizerBuilderFactory(RandomizerMode randomizerMode)
    {
        Mode = randomizerMode;
    }

    public RandomizerMode Mode { get; private set; }

    public void SetMode(RandomizerMode randomizerMode)
    {
        Mode = randomizerMode;
    }

    /// <summary>Sets the configuration used by <see cref="RandomizerMode.Custom"/>. It is copied.</summary>
    public void SetCustomMode(RandomizerConfiguration configuration)
    {
        _configurationMap[RandomizerMode.Custom] = configuration.Copy();
    }

    /// <summary>Builder configured for the current <see cref="Mode"/>.</summary>
    public RandomizerBuilder<T> GetBuilder<T>() where T : notnull
    {
        return new RandomizerBuilder<T>(_configurationMap[Mode]);
    }

    /// <summary>Builder configured for <paramref name="mode"/>, ignoring the current <see cref="Mode"/>.</summary>
    public RandomizerBuilder<T> GetBuilder<T>(RandomizerMode mode) where T : notnull
    {
        return new RandomizerBuilder<T>(_configurationMap[mode]);
    }
}
