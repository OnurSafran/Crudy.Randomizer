## About



## Usage

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
    ExcludeReturnedKeysForUniqueness = true
})
.Data(new Dictionary<int, int>(){
    { 1, 20 }, { 2, 30 }, { 3, 50 }
}).Build();

var values = randomizer.GetRandom(10);
```
all keys will be excluded after 3 iteration

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