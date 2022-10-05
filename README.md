# About

Easy to use weighted item randomizer for C#.

It can act like item pool. Value demonstrates possibility

It can act like marbles in a sack. Value demonstrates amount. When you draw, its amount will be reduced

Custom configuration is available

# Usage

*Dependency Injection*
-

``` c#
// Default is Pool
services.AddSingleton<RandomizerBuilderFactory>(new RandomizerBuilderFactory(RandomizerMode.Pool));
// Default is Sack
services.AddSingleton<RandomizerBuilderFactory>(new RandomizerBuilderFactory(RandomizerMode.Sack));
// Default is Custom
services.AddSingleton<RandomizerBuilderFactory>(new RandomizerBuilderFactory(new RandomizerConfiguration(){
    ExcludeOnDrawForUniqueness = true
}));
```

``` c#
ctor(RandomizerBuilderFactory factory){
    var builder = factory.GetBuilder<int>();
    var randomizer = builder.Data(...).Build();
    var value = randomizer.Draw(1).FirstOrDefault();
    
    var item = factory.GetBuilder<Item>().Data(...).Build().Draw(1).FirstOrDefault();
    
    var characters = factory.GetBuilder<Character>().Data(...).Build().Draw(5);
}
```

*Pool. Value demonstrates possibility*
-
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var value = randomizer.Draw(1).FirstOrDefault();
```
value can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Build();

var value = randomizer.Draw(1).FirstOrDefault();
```
value can be;
* %10 --> 1
* %15 --> 2
* %25 --> 3
* %50 --> 4

---
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Exclude(new List<int>(){
    4
}).Build();

var value = randomizer.Draw(1).FirstOrDefault();
```
value can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Exclude(new List<int>(){
    4
}).Build();
randomizer.Remove(3)

var value = randomizer.Draw(1).FirstOrDefault();
```
value can be;
* %40 --> 1
* %60 --> 2

---
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Pool).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var values = randomizer.Draw(10);
```
values count will be 10

values can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
*Sack. Value demonstrates amount*
-

``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
{ 1, 2 }, { 2, 2 }, { 3, 2 }
}).Build();

var values = randomizer.Draw(10);
```
all keys will be removed after 6 iterations

values count will be 6

values will contain two of every keys {1, 2, 3}

---
``` c#
var builder = new RandomizerBuilderFactory(RandomizerMode.Sack).GetBuilder<int>();
var randomizer = builder
.Data(new Dictionary<int, int>(){
{ 1, 5 }, { 2, 1 }
}).Build();

var values = randomizer.Draw(10);
```
all keys will be removed after 6 iterations

values count will be 6

values will include 5 times 1, 1 time 2

---
*Custom*
-

``` c#
var builder = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    ExcludeOnDrawForUniqueness = true
});
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var values = randomizer.Draw(10);
```
all keys will be excluded after 3 iterations

values count will be 3

values will contain one of every keys {1, 2, 3}

---
``` c#
var builder = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    ExcludeOnDrawForUniqueness = false
});
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 100 }, { 2, -100 }
}).Build();
randomizer.Add(2, 100);

var values = randomizer.Draw(10);
```
values count will be 10

values can be;
* %50 --> 1
* %50 --> 2

---
``` c#
var builder = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    AllowNegativeValues = true
});
var randomizer = builder
.Data(new Dictionary<int, int>(){
    { 1, 100 }, { 2, -100 }
}).Build();
randomizer.Add(2, 100);

var values = randomizer.Draw(10);
```
values count will be 10

values will contain only {1}

values can be;
* %100 --> 1

---