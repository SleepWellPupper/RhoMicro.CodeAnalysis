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
| 6  | An error is issued if more than 31 variant groups are defined. This is to allow groups to be modelled using a [Flags] enum.                                                                           |        | ×         |
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
| 25 | An error is issued for json serializable unions that are generic (transitively).                                                                                                                      |        | ×         |
| 26 | An error is issued for interface variants.                                                                                                                                                            |        | ×         |
| 27 | An error is issued for duplicate variant types.                                                                                                                                                       |        | ×         |
| 28 | An error is issued for static unions.                                                                                                                                                                 |        | ×         |
| 29 | An error is issued if the union implements the variant.                                                                                                                                               |        | ×         |
| 30 | An error is issued for `allows ref` variants.                                                                                                                                                         |        | ×         |
| 31 | /* A warning is issued for transitively generic unions. */                                                                                                                                            |        | ×         |
| 32 | An error is issued for variants that are the union itself.                                                                                                                                            |        | ×         |
| 33 | An error is issued for any non-interface base type (excluding `System.Object`, `System.ValueType`) implemented by the union.                                                                          |        | ×         |
| 34 | A warning is issued for `IsNullable = true` set on value type variants.                                                                                                                               |        | ×         |
| 35 | A error is issued if a variants of `Nullable<T>` and `T` are defined for the same union.                                                                                                              |        | ×         |
| 36 | A warning is issued for an explicitly set `ToStringSetting` if a `ToString` implementation is user provided.                                                                                          |        | ×         |
| 37 | If a `ToString` implementation is user provided, no conflicting implementation is emitted.                                                                                                            |        | ×         |
| 38 | An error is issued for duplicate variant names.                                                                                                                                                       |        | ×         |
| 39 |                                                                                                                                                                                                       |        | ×         |
| 40 |                                                                                                                                                                                                       |        | ×         |
| 41 |                                                                                                                                                                                                       |        | ×         |
| 42 |                                                                                                                                                                                                       |        | ×         |
| 43 |                                                                                                                                                                                                       |        | ×         |
| 44 |                                                                                                                                                                                                       |        | ×         |
| 45 |                                                                                                                                                                                                       |        | ×         |
| 46 |                                                                                                                                                                                                       |        | ×         |
| 47 |                                                                                                                                                                                                       |        | ×         |
| 48 |                                                                                                                                                                                                       |        | ×         |
| 49 |                                                                                                                                                                                                       |        | ×         |
| 50 |                                                                                                                                                                                                       |        | ×         |

## Breaking Changes

This is a non-exhaustive list of breaking changes to the previous version:

| id | change                                                                                                         | remediation / replacement                                                                                                          |
|----|----------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| 1  | `RelationAttribute` has been removed.                                                                          | Use explicit conversion through `TTo.Create(TFrom)`, where `TTo` and `TFrom` are unions.                                           |
| 2  | `UnionTypeFactoryAttribute` has been removed.                                                                  | Custom validation logic should be implemented through the partial `static partial void Validate(TVariant, bool, ref bool)` method. |
| 3  | `LayoutSetting` and `UnionTypeSettingsAttribute.Layout` have been removed.                                     | None - layouts are no longer customizable.                                                                                         |
| 4  | `ConstructorAccessibilitySetting` and `UnionTypeSettingsAttribute.ConstructorAccessibility` have been removed. | None - constructor accessibility is no longer customizable.                                                                        |
| 5  | `InterfaceMatchSetting` and `UnionTypeSettingsAttribute.InterfaceMatchSetting` have been removed.              | Use the `Switch` overloads to match interface implementations.                                                                     |
| 6  | `DiagnosticsLevelSettings` and `UnionTypeSettingsAttribute.DiagnosticsLevel` have been removed.                | Diagnostics should be configured through builtin mechanisms like `.editorconfig` or `#pragma warning disable`.                     |
| 7  | `MiscellaneousSettings` and `UnionTypeSettingsAttribute.Miscellaneous` have been removed.                      | Enable json converter generation through the `UnionTypeSettingsAttribute.IsJsonSerializable` property.                             |
| 8  | `UnionTypeSettingsAttribute.TypeDeclarationPreface` has been removed.                                          | Apply any comment or attribute annotations to your partial declaration of the union.                                               |
| 9  | `Match` methods have been removed.                                                                             | Use the new `Switch` method overloads.                                                                                             |
| 10 | All naming options from `UnionTypeSettings` have been removed.                                                 | None, open an issue on the github repository outlining your custom naming needs.                                                   |
| 11 | `StorageOption` and `UnionTypeAttribute.Storage` have been removed.                                            | None, storage is no longer customizable.                                                                                           |
| 12 | `UnionTypeAttribute.FactoryName` has been removed.                                                             | None, factory names are no longer customizable.                                                                                    |
| 13 | `UnionTypeOptions` has been removed.                                                                           |                                                                                                                                    |
| 14 |                                                                                                                |                                                                                                                                    |
| 15 |                                                                                                                |                                                                                                                                    |
| 16 |                                                                                                                |                                                                                                                                    |
| 17 |                                                                                                                |                                                                                                                                    |
| 18 |                                                                                                                |                                                                                                                                    |
| 19 |                                                                                                                |                                                                                                                                    |
| 20 |                                                                                                                |                                                                                                                                    |

- Relations have been removed
- Factories have been removed

## Notes

### TODO

- relations

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

