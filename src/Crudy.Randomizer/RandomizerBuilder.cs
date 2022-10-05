namespace Crudy.Randomizer; 

public class RandomizerBuilder<T> where T : notnull {
	private readonly Randomizer<T> _randomizer;

	public RandomizerBuilder() {
		_randomizer = new Randomizer<T>();
	}
	
	public RandomizerBuilder(RandomizerConfiguration randomizerConfiguration) {
		_randomizer = new Randomizer<T>(randomizerConfiguration);
	}

	public RandomizerBuilder<T> Data(Dictionary<T, int> dictionary) {
		_randomizer.Add(dictionary);
		return this;
	}

	public RandomizerBuilder<T> Exclude(List<T> excludeList) {
		_randomizer.AddExcludes(excludeList);
		return this;
	}

	public Randomizer<T> Build() {
		return _randomizer;
	}
}