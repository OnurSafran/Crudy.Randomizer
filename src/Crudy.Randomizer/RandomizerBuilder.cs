namespace Crudy.Randomizer; 

public class RandomizerBuilder<T> where T : notnull {
	private RandomizerConfiguration? _randomizerConfiguration = null;
	private Dictionary<T, int>? _dictionary = null;
	private List<T>? _excludedKeys = null;

	public RandomizerBuilder() {
	}
	
	public RandomizerBuilder(RandomizerConfiguration randomizerConfiguration) {
		_randomizerConfiguration = randomizerConfiguration;
	}

	public RandomizerBuilder<T> Configuration(RandomizerConfiguration randomizerConfiguration) {
		_randomizerConfiguration = randomizerConfiguration;
		return this;
	}

	public RandomizerBuilder<T> Data(Dictionary<T, int> dictionary) {
		_dictionary = dictionary;
		return this;
	}

	public RandomizerBuilder<T> Exclude(List<T> excludeList) {
		_excludedKeys = excludeList;
		return this;
	}

	public Randomizer<T> Build()
	{
		var randomizer = _randomizerConfiguration != null ? new Randomizer<T>(_randomizerConfiguration) : new Randomizer<T>();
		
		if(_dictionary != null)
			randomizer.Add(_dictionary);
		
		if(_excludedKeys != null)
			randomizer.AddExcludes(_excludedKeys);
		
		return randomizer;
	}
}