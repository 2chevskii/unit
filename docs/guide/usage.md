# Usage

Usage of the `Unit` type is very simple, here are some facts:

## Equality checks

- All `Unit`s are equal to each other, therefore
  - `Unit == Unit` is `true`
  - `Unit != Unit` is `false`
  - `Unit < or > Unit` is `false`
  - `Unit <= or >= Unit` is `true`
- `Equals(Unit)` method returns `true`
- `Equals(object)` method returns `false` when given any object which is not a `Unit`
- `CompareTo(Unit)` method returns `0`

## Hash code

`Unit.GetHashCode()` method always returns `804741542`

## String representation

`Unit.ToString()` method always returns `()`

## Namespace

Besides the fact, that all modern **C#** editors by now have an option to include `using` directives
automatically, you may need it anyways, so here it is:

```cs
using Dvchevskii.Unit;
```
