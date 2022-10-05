namespace Crudy.Randomizer;

public class RandomizerBuilderFactory
{
    private RandomizerMode _mode;
    
    private readonly Dictionary<RandomizerMode, RandomizerConfiguration> _configurationMap =
        new Dictionary<RandomizerMode, RandomizerConfiguration>()
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
        
        _mode = RandomizerMode.Custom;
    }
    
    public RandomizerBuilderFactory(RandomizerMode randomizerMode)
    {
        SetConfiguration(randomizerMode);
    }
    
    public void SetConfiguration(RandomizerMode randomizerMode)
    {
        _mode = randomizerMode;
    }
    
    public void SetCustomMode(RandomizerConfiguration configuration)
    {
        _configurationMap[RandomizerMode.Custom] = configuration;
    }

    public RandomizerBuilder<T> GetBuilder<T>() where T : notnull
    {
        return new RandomizerBuilder<T>(_configurationMap[_mode]);
    }

    public RandomizerBuilder<T> GetBuilder<T>(RandomizerConfiguration configuration) where T : notnull
    {
        return new RandomizerBuilder<T>(configuration);
    }
}