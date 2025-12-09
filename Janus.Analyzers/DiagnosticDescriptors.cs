// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Microsoft.CodeAnalysis;

internal static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor ToStringSettingIgnored { get; } = new(
        id: "RMJ0001",
        title: "`ToStringSetting` is ignored due to user defined `ToString` implementation",
        messageFormat: "`{0}` is ignored due to user defined `ToString` implementation in `{1}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnionMayNotBeRecordType { get; } = new(
        id: "RMJ0002",
        title: "Union may not be record type",
        messageFormat: "`{0}` may not be a record type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor GenericUnionsCannotBeJsonSerializable { get; } = new(
        id: "RMJ0003",
        title: "Generic unions cannot be json serializable",
        messageFormat: "`{0}` is a generic type and may therefore not be json serializable",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NoMoreThan31VariantGroupsMayBeDefined { get; } = new(
        id: "RMJ0004",
        title: "No more than 31 variant groups may be defined",
        messageFormat: "`{0}` defines {1} groups, but no more than 31 groups may be defined",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnionMayNotBeStatic { get; } = new(
        id: "RMJ0005",
        title: "Union may not be static",
        messageFormat: "`{0}` may not be static",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor VariantNamesMustBeUnique { get; } = new(
        id: "RMJ0006",
        title: "Variant names must be unique",
        messageFormat: "`{0}` already defines a variant with the name `{1}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor EnsureValidStructUnionState { get; } = new(
        id: "RMJ0007",
        title: "Ensure that struct unions have at least one struct or nullable reference type variant",
        messageFormat:
        "`{0}` is a struct union but does not have a struct or nullable reference type variant. In order to ensure the union is always in a correct state, a struct variant or nullable reference type should be provided. If no such variant is provided, the union should be a class",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InterfaceVariantIsExcludedFromConversionOperators { get; } = new(
        id: "RMJ0008",
        title: "Interface variants are excluded from conversion operator generation",
        messageFormat:
        "Variant `{0}` of union `{1}` is excluded from conversion operator generation because it is an interface",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor VariantTypesMustBeUnique { get; } = new(
        id: "RMJ0009",
        title: "Variant types must be unique",
        messageFormat: "`{0}` already defines a variant with the type `{1}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ObjectCannotBeUsedAsAVariant { get; } = new(
        id: "RMJ0010",
        title: "`object` cannot be used as a variant",
        messageFormat: "`object` cannot be used as a variant of `{0}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ValueTypeCannotBeUsedAsAVariantOfStructUnion { get; } = new(
        id: "RMJ0019",
        title: "`ValueType` cannot be used as a variant of struct union",
        messageFormat: "`ValueType` cannot be used as a variant of `{0}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnionCannotBeUsedAsVariantOfItself { get; } = new(
        id: "RMJ0012",
        title: "Union cannot be used as variant of itself",
        messageFormat: "`{0}` cannot be used as a variant of itself",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnionCannotExplicitlyDefineBaseType { get; } = new(
        id: "RMJ0013",
        title: "Union cannot explicitly define base type",
        messageFormat: "`{0}` cannot explicitly define base type `{1}`",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor PreferNullableStructOverIsNullable { get; } = new(
        id: "RMJ0014",
        title: "Prefer `Nullable<T>` over `IsNullable = true`",
        messageFormat:
        "Prefer `Nullable<{0}>` over `{1}` for variant `{2}`, as the `IsNullable` option only affects nullable reference type variants",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NullableVariantNotAllowedAlongWithNonNullableVariant { get; } = new(
        id: "RMJ0015",
        title: "`Nullable<T>` and `T` variants are not allowed for the same type `T`",
        messageFormat: "`{0}` cannot have both `Nullable<{1}>` and `{1}` variants",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor UnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute { get; } = new(
        id: "RMJ0016",
        title: "`UnionTypeSettingsAttribute` is ignored, as no `UnionTypeAttribute` has been applied",
        messageFormat: "`UnionTypeSettingsAttribute` on `{0}` is ignored, as no `UnionTypeAttribute` has been applied",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DuplicateVariantGroupNamesAreIgnored { get; } = new(
        id: "RMJ0017",
        title: "Duplicate variant group names are ignored",
        messageFormat: "The variant group `{0}` has been specified",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor ClassUnionsShouldBeSealed { get; } = new(
        id: "RMJ0018",
        title: "Class unions should be sealed",
        messageFormat: "The union `{0}` should be sealed",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor UnionCannotBeRefStruct { get; } = new(
        id: "RMJ0020",
        title: "Union cannot be ref struct",
        messageFormat: "Union `{0}` cannot be a ref struct",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
