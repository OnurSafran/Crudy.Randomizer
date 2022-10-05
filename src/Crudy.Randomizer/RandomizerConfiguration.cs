namespace Crudy.Randomizer; 

public class RandomizerConfiguration
{
	public bool AllowNegativeValues { get; set; } = true;
	public bool ClearZeros { get; set; } = true;
	public bool ExcludeReturnedKeysForUniqueness { get; set; } = false;
}