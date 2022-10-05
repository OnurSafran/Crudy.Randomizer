# About

It can act like item pool. Value demonstrates possibility

It can act like marbles in a sack. Value demonstrates amount. When you draw, its amount will be reduced

# Usage

*Pool. Value demonstrates possibility*
-
``` c#
var randomizer = new RandomizerBuilder<int>()
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var value = randomizer.GetRandom(1).First();
```
value can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
``` c#
var randomizer = new RandomizerBuilder<int>()
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Build();

var value = randomizer.GetRandom(1).First();
```
value can be;
* %10 --> 1
* %15 --> 2
* %25 --> 3
* %50 --> 4

---
``` c#
var randomizer = new RandomizerBuilder<int>()
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Exclude(new List<int>(){
    4
}).Build();

var value = randomizer.GetRandom(1).First();
```
value can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
``` c#
var randomizer = new RandomizerBuilder<int>()
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }, { 4, 100 }
}).Exclude(new List<int>(){
    4
}).Build();
randomizer.Remove(3)

var value = randomizer.GetRandom(1).First();
```
value can be;
* %40 --> 1
* %60 --> 2

---
``` c#
var randomizer = new RandomizerBuilder<int>()
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var values = randomizer.GetRandom(10);
```
values count will be 10

values can be;
* %20 --> 1
* %30 --> 2
* %50 --> 3

---
``` c#
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    ExcludeOnDrawForUniqueness = true
})
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var values = randomizer.GetRandom(10);
```
all keys will be excluded after 3 iterations

values count will be 3

values will contain one of every keys {1, 2, 3}

---
``` c#
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    AllowNegativeValues = false
})
.Data(new Dictionary<int, int>(){
    { 1, 100 }, { 2, -100 }
}).Build();
randomizer.Add(2, 100);

var values = randomizer.GetRandom(10);
```
values count will be 10

values can be;
* %50 --> 1
* %50 --> 2

---
``` c#
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    AllowNegativeValues = true
})
.Data(new Dictionary<int, int>(){
    { 1, 100 }, { 2, -100 }
}).Build();
randomizer.Add(2, 100);

var values = randomizer.GetRandom(10);
```
values count will be 10

values will contain only {1}

values can be;
* %100 --> 1

---
*Sack. Value demonstrates amount*
-

```
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    RemoveOnDraw = true
})
.Data(new Dictionary<int, int>(){
{ 1, 2 }, { 2, 2 }, { 3, 2 }
}).Build();

var values = randomizer.GetRandom(10);
```
all keys will be removed after 6 iterations

values count will be 6

values will contain two of every keys {1, 2, 3}

---
```
var randomizer = new RandomizerBuilder<int>(new RandomizerConfiguration(){
    RemoveOnDraw = true
})
.Data(new Dictionary<int, int>(){
{ 1, 5 }, { 2, 1 }
}).Build();

var values = randomizer.GetRandom(10);
```
all keys will be removed after 6 iterations

values count will be 6

values will include 5 times 1, 1 time 2