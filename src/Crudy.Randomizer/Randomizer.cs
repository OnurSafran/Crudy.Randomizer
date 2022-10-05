using System.Text;

namespace Crudy.Randomizer; 

public class Randomizer<T> where T : notnull
{
    private readonly Dictionary<T, int> _dictionary;
    private readonly List<T> _excludedKeys;
    private int _total;
    private readonly RandomizerConfiguration _randomizerConfiguration;

    public Randomizer()
    {
        _dictionary = new Dictionary<T, int>();
        _excludedKeys = new List<T>();
        _total = 0;
        _randomizerConfiguration = new RandomizerConfiguration();
    }
    
    public Randomizer(RandomizerConfiguration randomizerConfiguration)
    {
        _dictionary = new Dictionary<T, int>();
        _excludedKeys = new List<T>();
        _total = 0;
        _randomizerConfiguration = randomizerConfiguration;
    }

    public void Add(Dictionary<T, int> dictionary)
    {
        foreach (var entry in dictionary) Add(entry.Key, entry.Value);
    }

    public void Add(Randomizer<T> randomizer)
    {
        foreach (var entry in randomizer._dictionary) Add(entry.Key, entry.Value);
    }

    public void AddExistingKeys(Randomizer<T> randomizer) {
        foreach (var entry in randomizer._dictionary.Where(entry => _dictionary.ContainsKey(entry.Key)))
            Add(entry.Key, entry.Value);
    }

    public void Add(T key, int value)
    {
        if (value == 0)
            return;

        if (_dictionary.ContainsKey(key))
            Change(key, value);
        else
            Create(key, value);
    }

    public void Remove(T key)
    {
        if (!_dictionary.ContainsKey(key))
            return;

        if (_dictionary[key] > 0 && !_excludedKeys.Contains(key))
            _total -= _dictionary[key];

        _dictionary.Remove(key);
    }

    public void Remove(List<T> keys)
    {
        keys.ForEach(Remove);
    }

    public void AddExclude(T key) {
        if (_excludedKeys.Contains(key))
            return;
        
        _excludedKeys.Add(key);
        
        if(!_dictionary.ContainsKey(key))
            return;

        if(_dictionary[key] > 0)
            _total -= _dictionary[key];
    }

    public void AddExcludes(List<T> keys) {
        keys.ForEach(AddExclude);
    }

    public void RemoveExclude(T key) {
        if (!_excludedKeys.Contains(key))
            return;
        
        _excludedKeys.Remove(key);
        
        if(!_dictionary.ContainsKey(key))
            return;

        if(_dictionary[key] > 0)
            _total += _dictionary[key];
    }

    public void RemoveExcludes(List<T> keys) {
        keys.ForEach(RemoveExclude);
    }

    public void RemoveAllExcludes() {
        RemoveExcludes(_excludedKeys.ToList());
    }

    private void Create(T t, int value)
    {
        if (!_randomizerConfiguration.AllowNegativeValues && value < 0)
            return;
        
        _dictionary.Add(t, value);

        if (value > 0 && !_excludedKeys.Contains(t))
            _total += value;
    }

    private void Change(T t, int value)
    {
        ChangeTotal(t, value);

        ChangeValue(t, value);
    }

    private void ChangeTotal(T t, int value)
    {
        if (_excludedKeys.Contains(t))
            return;

        var tempTotal = _dictionary[t] + value;
        if (tempTotal < 0)
            tempTotal = 0;

        var tempValue = _dictionary[t] > 0 ? _dictionary[t] : 0;

        _total += (tempTotal - tempValue);
    }

    private void ChangeValue(T t, int value)
    {
        _dictionary[t] += value;

        if (_dictionary[t] < 0 && !_randomizerConfiguration.AllowNegativeValues)
            _dictionary[t] = 0;
        
        if (_dictionary[t] == 0 && _randomizerConfiguration.ClearZeros)
            _dictionary.Remove(t);
    }

    public string Print()
    {
        var stringBuilder = new StringBuilder();

        foreach (var entry in _dictionary)
        {
            stringBuilder.Append($"{entry.Key}:{entry.Value}");
            stringBuilder.Append(" , ");
        }

        stringBuilder.Append($"Total:{_total}");

        return stringBuilder.ToString();
    }

    private T? GetRandom()
    {
        int counter = 0, rand = Random.Shared.Next(0, _total);

        foreach (var entry in _dictionary)
        {
            if (entry.Value <= 0 || _excludedKeys.Contains(entry.Key))
                continue;

            counter += entry.Value;

            if (counter > rand)
            {
                return entry.Key;
            }
        }

        return default(T);
    }
    
    public List<T> GetRandom(int count)
    {
        var list = new List<T>();
        
        foreach (var i in Enumerable.Range(0, count))
        {
            var key = GetRandom();
            
            if(key?.Equals(default(T)) ?? true)
                continue;

            if(_randomizerConfiguration.ExcludeReturnedKeysForUniqueness)
                AddExclude(key);
            
            list.Add(key);
        }

        return list;
    }

    public Randomizer<T> Clone() {
        return new RandomizerBuilder<T>(_randomizerConfiguration).Data(_dictionary).Exclude(_excludedKeys).Build();
    }

    public static Randomizer<T> operator +(Randomizer<T> r1, Randomizer<T> r2)
    {
        var randomizer = new Randomizer<T>(r1._randomizerConfiguration);

        randomizer.Add(r1);
        randomizer.Add(r2);
        randomizer.AddExcludes(r1._excludedKeys);
        randomizer.AddExcludes(r2._excludedKeys);

        return randomizer;
    }
}