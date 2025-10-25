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

| id | description                                                                                 | issues | met (×/✓) |
|----|---------------------------------------------------------------------------------------------|--------|-----------|
| 1  | An error is issued if `Union` is not partial.                                               | #151   | ×         |
| 2  | Interfaces implemented by all variants are implemented by an adapter property on the union. | #40    | ×         |
| 3  | Conflicting interface member implementations on the adapter are implemented explicitly.     | #40    | ×         |
| 4  | An error is issued for `ref` like variants.                                                 |        | ×         |
| 4  | An error is issued for `record` unions.                                                     |        | ×         |
| 5  | A warning is issued for non-nullable reference variant on struct union.                     |        | ×         |
| 6  | An error is issued if more than 31 variant groups are defined.                              |        | ×         |
| 7  |                                                                                             |        | ×         |
| 8  |                                                                                             |        | ×         |
| 9  |                                                                                             |        | ×         |
| 10 |                                                                                             |        | ×         |

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

| id | signature                    | description                                                                          | conditions | issues | met (×/✓) |
|----|------------------------------|--------------------------------------------------------------------------------------|------------|--------|-----------|
| 1  | `public enum Variant : byte` | enumerates all variant names, names are mapped onto variant type references or aliae |            |        | ×         |
| 2  | `None = 0`                   | default value, used for detecting uninitialized unions                               |            |        | ×         |
| 3  |                              |                                                                                      |            |        | ×         |
| 4  |                              |                                                                                      |            |        | ×         |
| 5  |                              |                                                                                      |            |        | ×         |
| 6  |                              |                                                                                      |            |        | ×         |
| 7  |                              |                                                                                      |            |        | ×         |
| 8  |                              |                                                                                      |            |        | ×         |
| 9  |                              |                                                                                      |            |        | ×         |
| 10 |                              |                                                                                      |            |        | ×         |

### `Union.VariantOperations`

| id | signature                               | description                                                    | conditions | issues | met (×/✓) |
|----|-----------------------------------------|----------------------------------------------------------------|------------|--------|-----------|
| 1  | `public static class VariantOperations` | defines operations and extensions on `Union.Variant` instances |            |        | ×         |
| 2  | `public string Name { get; }`           | gets the name of the variant                                   |            |        | ×         |
| 3  | `public bool IsDefault { get; }`        | indicates if the variant is `Variant.None`                     |            |        | ×         |
| 4  | `public VariantGroups Groups { get; }`  | gets the groups containing the variant                         |            |        | ×         |
| 5  |                                         |                                                                |            |        | ×         |
| 6  |                                         |                                                                |            |        | ×         |
| 7  |                                         |                                                                |            |        | ×         |
| 8  |                                         |                                                                |            |        | ×         |
| 9  |                                         |                                                                |            |        | ×         |
| 10 |                                         |                                                                |            |        | ×         |

### `Union.VariantGroups`

| id | signature                                 | description                                  | conditions | issues | met (×/✓) |
|----|-------------------------------------------|----------------------------------------------|------------|--------|-----------|
| 1  | `[Flags] public enum VariantGroups : int` | enumerates all variant group names           |            |        | ×         |
| 2  | `None = 0`                                | default group used for unassociated variants |            |        | ×         |
| 3  |                                           |                                              |            |        | ×         |
| 4  |                                           |                                              |            |        | ×         |
| 5  |                                           |                                              |            |        | ×         |
| 6  |                                           |                                              |            |        | ×         |
| 7  |                                           |                                              |            |        | ×         |
| 8  |                                           |                                              |            |        | ×         |
| 9  |                                           |                                              |            |        | ×         |
| 10 |                                           |                                              |            |        | ×         |

### `Union.VariantGroupsOperations`

| id | signature                                     | description                                                          | conditions | issues | met (×/✓) |
|----|-----------------------------------------------|----------------------------------------------------------------------|------------|--------|-----------|
| 1  | `public static class VariantGroupsOperations` | defines operations and extensions on `Union.VariantGroups` instances |            |        | ×         |
| 2  | `public bool HasFlag(VariantGroups flag)`     | defines operations and extensions on `Union.VariantGroups` instances |            |        | ×         |
| 3  | `public string Name { get; }`                 | gets the name of the variant group                                   |            |        | ×         |
| 4  | `public bool IsDefault`                       | indicates if the variant group is `VariantGroups.None`               |            |        | ×         |
| 5  |                                               |                                                                      |            |        | ×         |
| 6  |                                               |                                                                      |            |        | ×         |
| 7  |                                               |                                                                      |            |        | ×         |
| 8  |                                               |                                                                      |            |        | ×         |
| 9  |                                               |                                                                      |            |        | ×         |
| 10 |                                               |                                                                      |            |        | ×         |

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

