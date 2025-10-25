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
| 5  | An error is issued if `Union` is not partial.                                               | #151   | ×         |
| 6  | The value may be accessed as `object`.                                                      | #146   | ×         |
| 7  | Interfaces implemented by all variants are implemented by an adapter property on the union. | #40    | ×         |
| 8  | Conflicting interface member implementations on the adapter are implemented explicitly.     | #40    | ×         |

## APIs

### `Union`

| id | signature                                                                                                         | description                                                                                                                                | conditions   | issues | met (×/✓) |
|----|-------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|--------------|--------|-----------|
| 1  | `T Switch<T>(Func<Variant0, T>, Func<Variant1, T>)`                                                               | projects the value onto `T`, cases are obligatory                                                                                          | none         |        | ×         |
| 2  | `T Switch<T>(Func<T>, Func<Variant0, T>? = null, Func<Variant1, T>? = null)`                                      | projects the value onto `T`, cases are optional, default case is obligatory                                                                | > 1 variants |        | ×         |
| 3  | `T Switch<T>(T, Func<Variant0, T>? = null, Func<Variant1, T>? = null)`                                            | projects the value onto `T`, cases are optional, default projection is obligatory                                                          | > 1 variants |        | ×         |
| 4  | `T Switch<T, TState>(TState, Func<Variant0, TState, T>, Func<Variant1, TState, T>)`                               | projects the value onto `T`, cases are obligatory, state is obligatory                                                                     | none         |        | ×         |
| 5  | `T Switch<T, TState>(TState, Func<TState>, Func<Variant0, TState, T>? = null, Func<Variant1, TState, T>? = null)` | projects the value onto `T`, cases are optional, default case is obligatory, state is obligatory                                           | > 1 variants |        | ×         |
| 6  | `T Switch<T, TState>(TState, T, Func<Variant0, TState, T>? = null, Func<Variant1, TState, T>? = null)`            | projects the value onto `T`, cases are optional, default projection is obligatory, state is obligatory                                     | > 1 variants |        | ×         |
| 7  | `void Switch(Action<Variant0>, Action<Variant1>)`                                                                 | handles the value, cases are obligatory                                                                                                    | none         |        | ×         |
| 8  | `void Switch(Action, Action<Variant0>? = null, Action<Variant1>? = null)`                                         | handles the value, cases are optional, default case is obligatory                                                                          | > 1 variants |        | ×         |
| 9  | `void Switch<TState>(TState, Action<Variant0, TState>, Action<Variant1, TState>)`                                 | handles the value, cases are obligatory, state is obligatory                                                                               | none         |        | ×         |
| 10 | `void Switch<TState>(TState, Action<TState> , Action<Variant0, TState>?, Action<Variant1, TState>?)`              | handles the value, cases are optional, default case is obligatory, state is obligatory                                                     | > 1 variants |        | ×         |
| 11 | `Union Create<TValue>(TValue)`                                                                                    | wraps a value in a union, is polymorphically sensitive (allows for variant instances typed to their superclass), accepts `Union` instances | none         | #153   | ×         |
| 12 | `bool TryCreate<TValue>(TValue, out Union union)`                                                                 | wraps a value in a union, is polymorphically sensitive (allows for variant instances typed to their superclass), accepts `Union` instances | none         | #153   | ×         |
| 13 | `ToObject`                                                                                                        |                                                                                                                                            |              |        | ×         |
| 14 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 15 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 16 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 17 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 18 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 19 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 20 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 21 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 22 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 23 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 24 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 25 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 26 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 27 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 28 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 29 |                                                                                                                   |                                                                                                                                            |              |        | ×         |
| 30 |                                                                                                                   |                                                                                                                                            |              |        | ×         |

### `Union.Variant`

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

### `Union.VariantGroup`

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

