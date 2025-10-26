# New UnionsGenerator Design

## Terminology

The term `union` refers to a tagged union or sum type, or an instance thereof.
It is able to represent an instance of a type present in its set of representable types.

The term `Union` refers to a nonspecific generated type implementing the defined union.

The term `variant` refers to a type representable by a union.
Unless improperly initialized, a union is always representing an instance of one of its variants.

The term `value` refers to the variant instance being represented by a union.

The term `adapter` refers to the type adapting the union onto interfaces implemented by all its variants.

## Requirements

| id | description                                                                                                                                                                                           | issues | met (×/✓) |
|----|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------|-----------|
| 1  | An error is issued if `Union` is not partial.                                                                                                                                                         | #151   | ×         |
| 2  | Interfaces implemented by all variants are implemented by an adapter property on the union.                                                                                                           | #40    | ×         |
| 3  | Conflicting interface member implementations on the adapter are implemented explicitly.                                                                                                               | #40    | ×         |
| 4  | An error is issued for `ref` like variants.                                                                                                                                                           |        | ×         |
| 4  | An error is issued for `record` unions.                                                                                                                                                               |        | ×         |
| 5  | A warning is issued for non-nullable reference variant on struct union.                                                                                                                               |        | ×         |
| 6  | An error is issued if more than 31 variant groups are defined. This is to allow groups to be modelled using an int backed [Flags] enum.                                                               |        | ×         |
| 7  | Variant group names have diagnostics mapped onto their definition in the attribute usage.                                                                                                             |        | ×         |
| 8  | Variant names have diagnostics mapped onto their definition in the attribute usage.                                                                                                                   |        | ×         |
| 9  | An error is issued if more than 255 variants are defined. This is to allow variants to be modelled using a byte backed enum.                                                                          |        | ×         |
| 10 | An option is provided to box managed struct variants.                                                                                                                                                 |        | ×         |
| 11 | Managed struct variants are stored in dedicated fields by default.                                                                                                                                    |        | ×         |
| 12 | An error is issued for static variants.                                                                                                                                                               |        | ×         |
| 13 | An error is issued for conflicting variant names.                                                                                                                                                     |        | ×         |
| 14 | An error is issued if no unmanaged, managed struct or nullable reference variant is defined for a struct union. This is to prevent struct unions to be left in an invalid state when uninitialized.   |        | ×         |
| 15 | Variants are ordered by unmanaged, managed struct, nullable reference, reference, fully qualified type name. This is to force the default variant to be valid when the struct union is uninitialized. |        | ×         |
| 16 | If exactly one variant is defined, an implicit conversion to that variant is defined.                                                                                                                 |        | ×         |
| 17 | If multiple variants are defined, an explicit conversion to each variant is defined.                                                                                                                  |        | ×         |
| 18 | If a validation method is implemented, then an explicit conversion from the validated variant is defined.                                                                                             |        | ×         |
| 19 | If a validation method is not implemented, then an implicit conversion from the variant is defined.                                                                                                   |        | ×         |
| 20 |                                                                                                                                                                                                       |        | ×         |
| 21 |                                                                                                                                                                                                       |        | ×         |
| 22 |                                                                                                                                                                                                       |        | ×         |
| 23 |                                                                                                                                                                                                       |        | ×         |
| 24 |                                                                                                                                                                                                       |        | ×         |
| 25 |                                                                                                                                                                                                       |        | ×         |
| 26 |                                                                                                                                                                                                       |        | ×         |
| 27 |                                                                                                                                                                                                       |        | ×         |
| 28 |                                                                                                                                                                                                       |        | ×         |
| 29 |                                                                                                                                                                                                       |        | ×         |
| 30 |                                                                                                                                                                                                       |        | ×         |

## APIs

### `Union`

| id | signature                                                                                                                | description                                                                                                                                | conditions    | issues | met (×/✓) |
|----|--------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|---------------|--------|-----------|
| 1  | `public T Switch<T>(Func<Variant0, T>, Func<Variant1, T>)`                                                               | projects the value onto `T`, cases are obligatory                                                                                          |               |        | ×         |
| 2  | `public T Switch<T>(Func<T>, Func<Variant0, T>? = null, Func<Variant1, T>? = null)`                                      | projects the value onto `T`, cases are optional, default case is obligatory                                                                | \> 1 variants |        | ×         |
| 3  | `public T Switch<T>(T, Func<Variant0, T>? = null, Func<Variant1, T>? = null)`                                            | projects the value onto `T`, cases are optional, default projection is obligatory                                                          | \> 1 variants |        | ×         |
| 4  | `public T Switch<T, TState>(TState, Func<Variant0, TState, T>, Func<Variant1, TState, T>)`                               | projects the value onto `T`, cases are obligatory, state is obligatory                                                                     |               |        | ×         |
| 5  | `public T Switch<T, TState>(TState, Func<TState>, Func<Variant0, TState, T>? = null, Func<Variant1, TState, T>? = null)` | projects the value onto `T`, cases are optional, default case is obligatory, state is obligatory                                           | \> 1 variants |        | ×         |
| 6  | `public T Switch<T, TState>(TState, T, Func<Variant0, TState, T>? = null, Func<Variant1, TState, T>? = null)`            | projects the value onto `T`, cases are optional, default projection is obligatory, state is obligatory                                     | \> 1 variants |        | ×         |
| 7  | `public void Switch(Action<Variant0>, Action<Variant1>)`                                                                 | handles the value, cases are obligatory                                                                                                    |               |        | ×         |
| 8  | `public void Switch(Action, Action<Variant0>? = null, Action<Variant1>? = null)`                                         | handles the value, cases are optional, default case is obligatory                                                                          | \> 1 variants |        | ×         |
| 9  | `public void Switch<TState>(TState, Action<Variant0, TState>, Action<Variant1, TState>)`                                 | handles the value, cases are obligatory, state is obligatory                                                                               |               |        | ×         |
| 10 | `public void Switch<TState>(TState, Action<TState> , Action<Variant0, TState>?, Action<Variant1, TState>?)`              | handles the value, cases are optional, default case is obligatory, state is obligatory                                                     | \> 1 variants |        | ×         |
| 11 | `public Union Create<TValue>(TValue)`                                                                                    | wraps a value in a union, is polymorphically sensitive (allows for variant instances typed to their superclass), accepts `Union` instances |               | #153   | ×         |
| 12 | `public bool TryCreate<TValue>(TValue, out Union union)`                                                                 | wraps a value in a union, is polymorphically sensitive (allows for variant instances typed to their superclass), accepts `Union` instances |               | #153   | ×         |
| 13 | `public object Value { get; }`                                                                                           | gets the underlying value, boxing it if it is a value type                                                                                 |               | #146   | ×         |
| 14 | `public Union.Variant Variant { get; }`                                                                                  | gets the variant                                                                                                                           |               |        | ×         |
| 15 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 16 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 17 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 18 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 19 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 20 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 21 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 22 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 23 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 24 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 25 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 26 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 27 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 28 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 29 |                                                                                                                          |                                                                                                                                            |               |        | ×         |
| 30 |                                                                                                                          |                                                                                                                                            |               |        | ×         |

### `Union.Variant`

| id | signature                                              | description                                            | conditions | issues | met (×/✓) |
|----|--------------------------------------------------------|--------------------------------------------------------|------------|--------|-----------|
| 1  | `public readonly struct Variant`                       | represents a variant and enumerates all valid variants |            |        | ×         |
| 2  | `public static Variant None { get; }`                  | default value, used for detecting uninitialized unions |            |        | ×         |
| 3  | `public static Variant Variant0 { get; }`              | enumeration of variants                                |            |        | ×         |
| 4  | `public static ImmutableArray<Variant> GetAllValues()` | gets an array containing all available variant groups  |            |        | ×         |
| 5  | `public string Name { get; }`                          | gets the name of the variant                           |            |        | ×         |
| 6  | `public Type Type { get; }`                            | gets the `System.Type` of the variant                  |            |        | ×         |
| 7  | `public VariantGroups Groups { get; }`                 | gets the variant groups containing this variant        |            |        | ×         |
| 8  | `public bool IsDefault { get; }`                       | indicates whether the variant is equal to `None`       |            |        | ×         |
| 9  |                                                        |                                                        |            |        | ×         |
| 10 |                                                        |                                                        |            |        | ×         |

### `Union.VariantGroups`

| id | signature                                                               | description                                                             | conditions | issues | met (×/✓) |
|----|-------------------------------------------------------------------------|-------------------------------------------------------------------------|------------|--------|-----------|
| 1  | `public readonly struct VariantGroups`                                  | represents a variant group and enumerates all valid variant groups      |            |        | ×         |
| 2  | `public static VariantGroups None { get; }`                             | default group used for unassociated variants                            |            |        | ×         |
| 3  | `public static VariantGroups VariantGroup0 { get; }`                    | enumeration of variant groups                                           |            |        | ×         |
| 4  | `public static ImmutableArray<VariantGroups> GetAllValues()`            | gets an array containing all available variant groups                   |            |        | ×         |
| 5  | `public bool Contains(VariantGroups groups)`                            | indicates whether groups is contained in the current group              |            |        | ×         |
| 6  | `public bool IsDefault { get; }`                                        | indicates whether the variant group is equal to `None`                  |            |        | ×         |
| 7  | `public static VariantGroups operator \|(VariantGroups, VariantGroups)` | bitwise or operator for combining variant groups                        |            |        | ×         |
| 8  | `public int IndividualGroupsCount { get; }`                             | gets the amount of individual groups combined in this group             |            |        | ×         |
| 9  | `public void GetIndividualGroups(Span<VariantGroups> buffer)`           | populates a buffer with the individual groups combined in this group    |            |        | ×         |
| 10 | `public ImmutableArray<VariantGroups> GetIndividualGroups()`            | gets an array containing the individual groups combined in this group   |            |        | ×         |
| 11 | `public string Name { get; }`                                           | gets the name of or combined names of the groups combined in this group |            |        | ×         |
| 12 |                                                                         |                                                                         |            |        | ×         |
| 13 |                                                                         |                                                                         |            |        | ×         |
| 14 |                                                                         |                                                                         |            |        | ×         |
| 15 |                                                                         |                                                                         |            |        | ×         |
| 16 |                                                                         |                                                                         |            |        | ×         |
| 17 |                                                                         |                                                                         |            |        | ×         |
| 18 |                                                                         |                                                                         |            |        | ×         |
| 19 |                                                                         |                                                                         |            |        | ×         |
| 20 |                                                                         |                                                                         |            |        | ×         |

## Notes

### TODO

- Relations

### Table Template

| id | signature | description | conditions | issues | met (×/✓) |
|----|-----------|-------------|------------|--------|-----------|
| 1  |           |             |            |        | ×         |
| 2  |           |             |            |        | ×         |
| 3  |           |             |            |        | ×         |
| 4  |           |             |            |        | ×         |
| 5  |           |             |            |        | ×         |
| 6  |           |             |            |        | ×         |
| 7  |           |             |            |        | ×         |
| 8  |           |             |            |        | ×         |
| 9  |           |             |            |        | ×         |
| 10 |           |             |            |        | ×         |

