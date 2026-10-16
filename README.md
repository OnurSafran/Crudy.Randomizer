# Crudy.Randomizer

[![NuGet](https://img.shields.io/nuget/v/Crudy.Randomizer.svg)](https://www.nuget.org/packages/Crudy.Randomizer)
[![CI](https://github.com/OnurSafran/Crudy.Randomizer/actions/workflows/ci.yml/badge.svg)](https://github.com/OnurSafran/Crudy.Randomizer/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/LICENSE)

Easy to use weighted randomizer for C#.

- **Pool**: weights are chances. Drawing never changes them. Good for loot tables and spawn rates.
- **Sack**: weights are amounts, like marbles in a sack. Each draw takes one out. Good for decks, bags and limited stock.
- **Custom**: mix the behaviours yourself, e.g. unique winners only.

Targets .NET 8 and .NET 10. Free to use in any project, including commercial ones ([MIT](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/LICENSE)).

## Contents

- [Installation](#installation)
- [Quick start](#quick-start)
- [Modes](#modes)
- [Configuration](#configuration)
- [Drawing](#drawing)
- [Changing weights and excluding keys](#changing-weights-and-excluding-keys)
- [Combining randomizers](#combining-randomizers)
- [Reproducible draws and custom key equality](#reproducible-draws-and-custom-key-equality)
- [Dependency injection](#dependency-injection)
- [Thread safety](#thread-safety)
- [Examples](#examples)
- [API overview](#api-overview)
- [Building and testing](#building-and-testing)
- [Releasing](#releasing)
- [License](#license)

## Installation

```bash
dotnet add package Crudy.Randomizer
```

## Quick start

```c#
using Crudy.Randomizer;

var lootTable = new RandomizerBuilder<string>()
    .Data(new Dictionary<string, int>()
    {
        { "Gold", 60 },
        { "Potion", 30 },
        { "Rare Sword", 9 },
        { "Legendary Crown", 1 }
    })
    .Build();

string? drop = lootTable.Draw();          // one item
IList<string> drops = lootTable.Draw(10); // ten items
```

## Modes

`RandomizerBuilderFactory` hands out builders preconfigured for a mode.

### Pool: weights are chances

```c#
var randomizer = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>()
    .Data(new Dictionary<int, int>() { { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 } })
    .Build();

var values = randomizer.Draw(10); // always 10 values
```

| Key | Weight | Chance |
|-----|-------:|-------:|
| 1   | 20     | 10%    |
| 2   | 30     | 15%    |
| 3   | 50     | 25%    |
| 4   | 100    | 50%    |

Weights don't have to add up to 100. Each key's chance is its weight divided by the total.

### Sack: weights are amounts

```c#
var randomizer = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<int>()
    .Data(new Dictionary<int, int>() { { 1, 5 }, { 2, 1 } })
    .Build();

var values = randomizer.Draw(10);
```

Only 6 marbles are in the sack, so `values` has 6 items: five `1`s and one `2`, in random order.
`Draw(count)` stops early when nothing is left.

### Custom

```c#
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration()
    {
        ExcludeOnDrawForUniqueness = true
    })
    .Data(new Dictionary<int, int>() { { 1, 20 }, { 2, 30 }, { 3, 50 } })
    .Build();

var values = randomizer.Draw(10); // 1, 2 and 3 once each, in weighted order
```

Pass the configuration to the builder, or use `new RandomizerBuilderFactory(configuration)` to make it the factory's default.

## Configuration

| Setting                      | Default | Pool  | Sack  | What it does |
|------------------------------|---------|-------|-------|--------------|
| `AllowNegativeValues`        | `true`  | `true`  | `false` | Keep weights below zero. When `false`, a weight that would go negative becomes `0`. Keys with weight ≤ 0 are never drawn. |
| `ClearZeros`                 | `true`  | `true`  | `true`  | Remove a key as soon as its weight is `0`. When `false`, the key stays with weight `0`. |
| `ExcludeOnDrawForUniqueness` | `false` | `false` | `false` | Exclude a key after it's drawn, so each key is drawn at most once. Its weight is kept. |
| `RemoveOnDraw`               | `false` | `false` | `true`  | Lower a key's weight by 1 each time it's drawn. |

The configuration is copied when passed in, so changing it afterwards doesn't affect builders or randomizers that already have it.

How `AllowNegativeValues` changes things:

```c#
var data = new Dictionary<int, int>() { { 1, 100 }, { 2, -100 } };

// AllowNegativeValues = false: 2 is clamped to 0, then raised to 100  -> 1: 50%, 2: 50%
// AllowNegativeValues = true:  2 stays at -100, then raised to 0      -> 1: 100%
randomizer.Add(2, 100);
```

## Drawing

```c#
IList<T> values = randomizer.Draw(5);  // up to 5 keys
T? value = randomizer.Draw();          // one key, or default when nothing can be drawn

if (randomizer.TryDraw(out var key))   // one key, or false when nothing can be drawn
{
    // use key
}
```

`Draw()` returns `default` when nothing can be drawn. For value types that's ambiguous: `0`, or the first member of an enum, is also a valid key. Use `TryDraw` to tell them apart:

```c#
enum Rarity { Common, Rare, Epic }

while (sack.TryDraw(out var rarity))
    Open(rarity);           // Common is a real draw here

sack.Draw();                // returns Common (default) when empty, which is misleading
```

## Changing weights and excluding keys

```c#
randomizer.Add("Gold", 10);                   // add 10 to Gold's weight (creates the key if needed)
randomizer.Add("Gold", -5);                   // lower it by 5
randomizer.Add(new Dictionary<string, int>()  // add several; duplicate keys are summed
{
    { "Gem", 2 }, { "Key", 1 }
});
randomizer.Remove("Key");                     // remove the key entirely
randomizer.Remove(new[] { "Gem", "Gold" });

randomizer.AddExclude("Dragon");              // keep the weight but stop drawing it
randomizer.AddExcludes(new[] { "Boss", "Chest" });
randomizer.RemoveExclude("Dragon");           // drawable again
randomizer.RemoveAllExcludes();
```

Keys can be excluded before they're added; the exclude applies once they are. The builder takes excludes too:

```c#
var randomizer = new RandomizerBuilder<int>()
    .Data(new Dictionary<int, int>() { { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 } })
    .Exclude(new[] { 4 })
    .Build();
randomizer.Remove(3);

// 1: 40%, 2: 60%
```

`Clone()` gives an independent copy with the same weights, excludes, configuration, random source and comparer.

## Combining randomizers

Each returns a new randomizer and leaves both inputs unchanged. Shared keys get their weights summed.

```c#
// x = { 1:10, 2:10 }
// y = { 2:10, 3:10 }

x.Union(y);           // x + y  --> { 1:10, 2:20, 3:10 }  all keys, excludes of both
x.UpdateExisting(y);  // x * y  --> { 1:10, 2:20 }        only x's keys, y's new keys ignored, x's excludes
x.Intersect(y);       // x & y  --> { 2:20 }              only keys in both, excludes of both for those keys
```

The result uses the **left** randomizer's configuration, random source and comparer, so `x + y` and `y + x` can behave differently when their configurations differ.

## Reproducible draws and custom key equality

```c#
var randomizer = new RandomizerBuilder<string>()
    .Random(new Random(42))                     // seeded: same draws every run (default is Random.Shared)
    .Comparer(StringComparer.OrdinalIgnoreCase) // "sword" and "Sword" are the same key
    .Data(new Dictionary<string, int>() { { "sword", 10 }, { "shield", 20 } })
    .Build();
```

Both can also be passed straight to the constructor: `new Randomizer<string>(configuration, random, comparer)`.

## Dependency injection

```c#
// Pool by default
services.AddSingleton(new RandomizerBuilderFactory(RandomizerMode.Pool));

// or Sack by default
services.AddSingleton(new RandomizerBuilderFactory(RandomizerMode.Sack));

// or your own configuration by default
services.AddSingleton(new RandomizerBuilderFactory(new RandomizerConfiguration()
{
    ExcludeOnDrawForUniqueness = true
}));
```

```c#
public class LootService(RandomizerBuilderFactory factory)
{
    public IList<Item> OpenChest(Dictionary<Item, int> table) =>
        factory.GetBuilder<Item>().Data(table).Build().Draw(3);

    public IList<Card> DealHand(IEnumerable<Card> deck) =>
        factory.GetBuilder<Card>(RandomizerMode.Sack)   // a specific mode, ignoring the default
            .Data(deck.Select(card => KeyValuePair.Create(card, 1)))
            .Build()
            .Draw(5);
}
```

## Thread safety

A `Randomizer<T>` is not thread-safe: use one per thread or lock around it.
A `RandomizerBuilderFactory` can be shared (e.g. as a DI singleton) once it's configured; call `SetMode` and `SetCustomMode` only at startup.

## Examples

[`samples/Crudy.Randomizer.Samples`](https://github.com/OnurSafran/Crudy.Randomizer/tree/master/samples/Crudy.Randomizer.Samples) is a console app with runnable examples:

| Name       | Example |
|------------|---------|
| `loot`     | [Loot table (Pool mode)](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/LootTableExample.cs) |
| `sack`     | [Marbles in a sack (Sack mode)](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/MarbleSackExample.cs) |
| `deck`     | [Shuffling a deck of cards](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/DeckShuffleExample.cs) |
| `raffle`   | [Raffle with unique winners](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/RaffleExample.cs) |
| `exclude`  | [Temporarily excluding keys](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/ExclusionExample.cs) |
| `combine`  | [Combining randomizers](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/CombiningExample.cs) |
| `seed`     | [Reproducible draws with a seed](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/SeededExample.cs) |
| `comparer` | [Case-insensitive keys](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/ComparerExample.cs) |
| `trydraw`  | [TryDraw with value-type keys](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/samples/Crudy.Randomizer.Samples/Examples/TryDrawExample.cs) |

Run all of them, or one by name:

```bash
dotnet run --project samples/Crudy.Randomizer.Samples
```

```bash
dotnet run --project samples/Crudy.Randomizer.Samples -- deck
```

Sample output:

```text
=== Marbles in a sack (Sack mode) ===
Before: Red:3 , Blue:2 , Green:1 , Total:6
Picked: Red, Green
Left:   Red:2 , Blue:2 , Total:4
Rest:   Red, Blue, Red, Blue
Empty:  Total:0

=== Combining randomizers ===
base:          Gold:60 , Potion:40 , Total:100
event:         Potion:20 , Pumpkin:30 , Total:50
base + event:  Gold:60 , Potion:60 , Pumpkin:30 , Total:150   (all keys)
base * event:  Gold:60 , Potion:60 , Total:120   (base keys only)
base & event:  Potion:60 , Total:60   (shared keys only)
```

## API overview

| Type | Members |
|------|---------|
| `RandomizerBuilder<T>` | `Configuration`, `Data`, `Exclude`, `Random`, `Comparer`, `Build` |
| `RandomizerBuilderFactory` | `Mode`, `SetMode`, `SetCustomMode`, `GetBuilder<T>()`, `GetBuilder<T>(mode)` |
| `IReadOnlyRandomizer<T>` | `Draw()`, `Draw(count)`, `TryDraw` |
| `IRandomizer<T>` | everything above, plus `Add`, `Remove`, `AddExclude(s)`, `RemoveExclude(s)`, `RemoveAllExcludes` |
| `Randomizer<T>` | everything above, plus `Clone`, `Union` (`+`), `UpdateExisting` (`*`), `Intersect` (`&`), `Print` / `ToString` |

`IReadOnlyRandomizer<T>` can't change weights directly, but a draw can still consume or exclude keys when `RemoveOnDraw` or `ExcludeOnDrawForUniqueness` is on.

## Building and testing

Requires the .NET 10 SDK (see [`global.json`](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/global.json)).

```bash
dotnet test
```

```bash
dotnet pack src/Crudy.Randomizer -c Release -o nupkg
```

## Releasing

[CI](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/.github/workflows/ci.yml) builds, tests and packs every push and pull request.
Pushing a tag like `v1.2.0` also publishes version `1.2.0` (with symbols) to nuget.org:

```bash
git tag v1.2.0
```

```bash
git push origin v1.2.0
```

This needs a `NUGET_API_KEY` repository secret: create an API key on nuget.org scoped to `Crudy.Randomizer` with push permission.

## License

[MIT](https://github.com/OnurSafran/Crudy.Randomizer/blob/master/LICENSE) © 2022-2026 Onur Safran. You can use, copy, modify and distribute it freely, including in commercial and closed-source projects, as long as the copyright and license notice is kept.
