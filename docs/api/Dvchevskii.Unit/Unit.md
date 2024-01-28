# Struct `Unit`

```cs
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
```

## Static fields

### Default

```cs
public static readonly Unit Default;
```

Static instance of the [Unit](#struct-unit) type. Use this whenever possible

## Operators

### ==(Unit, Unit) {#op-eq-unit-unit}

```cs
public static bool operator ==(Unit lhs, Unit rhs);
```

### !=(Unit, Unit) {#op-noneq-unit-unit}

```cs
public static bool operator !=(Unit lhs, Unit rhs);
```

### >(Unit, Unit) {#op-gt-unit-unit}

```cs
public static bool operator >(Unit lhs, Unit rhs);
```

### <(Unit, Unit) {#op-lt-unit-unit}

```cs
public static bool operator <(Unit lhs, Unit rhs);
```

### >=(Unit, Unit) {#op-gteq-unit-unit}

```cs
public static bool operator >=(Unit lhs, Unit rhs);
```

### <=(Unit, Unit) {#op-lteq-unit-unit}

```cs
public static bool operator <=(Unit lhs, Unit rhs);
```

## Instance methods

### ToString()

```cs
public override string ToString();
```

### Equals(object)

```cs
public override bool Equals(object obj);
```

### GetHashCode()

```cs
public override int GetHashCode();
```

### Equals(Unit)

```cs
public bool Equals(Unit other);
```

### CompareTo(Unit)

```cs
public bool CompareTo(Unit other);
```
