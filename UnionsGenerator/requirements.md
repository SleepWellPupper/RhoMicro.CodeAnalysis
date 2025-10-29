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
| 6  | An error is issued if more than 63 variant groups are defined. This is to allow groups to be modelled using a [Flags] enum.                                                                           |        | ×         |
| 7  | Variant group names have diagnostics mapped onto their definition in the attribute usage.                                                                                                             |        | ×         |
| 8  | Variant names have diagnostics mapped onto their definition in the attribute usage.                                                                                                                   |        | ×         |
| 9  | The backing type of the `VariantKind` is chosen as the smallest integral datatype able to accommodate all variants.                                                                                   |        | ×         |
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
| 20 | Documentation comments are emitted for every generated member.                                                                                                                                        |        | ×         |
| 21 | Parameter lists are wrapped and indented.                                                                                                                                                             |        | ×         |
| 22 | The backing type of the `VariantGroupKinds` is chosen as the smallest integral datatype able to accommodate all variants groups.                                                                      |        | ×         |
| 23 | `Union` methods are grouped by the variant they are specific to.                                                                                                                                      |        | ×         |
| 24 | Variant group names are ordered alphabetically, except for the default group `None`, which must always be the first element.                                                                          |        | ×         |
| 25 |                                                                                                                                                                                                       |        | ×         |
| 26 |                                                                                                                                                                                                       |        | ×         |
| 27 |                                                                                                                                                                                                       |        | ×         |
| 28 |                                                                                                                                                                                                       |        | ×         |
| 29 |                                                                                                                                                                                                       |        | ×         |
| 30 |                                                                                                                                                                                                       |        | ×         |

## Notes

### TODO

- relations
- json support

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

