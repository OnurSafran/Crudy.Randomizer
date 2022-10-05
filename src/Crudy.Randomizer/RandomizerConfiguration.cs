namespace Crudy.Randomizer; 

public class RandomizerConfiguration
{
	public bool AllowNegativeValues { get; set; } = true;
	public bool ClearZeros { get; set; } = true;
	public bool ExcludeOnDrawForUniqueness { get; set; } = false;
	public bool RemoveOnDraw { get; set; } = false;
}