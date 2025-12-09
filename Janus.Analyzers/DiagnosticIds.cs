// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

/// <summary>
/// Provides access to diagnostic ids.
/// </summary>
public static class DiagnosticIds
{
#pragma warning disable CS1591
    public const String ToStringSettingIgnored = "RMJ0001";
    public const String UnionMayNotBeRecordType = "RMJ0002";
    public const String GenericUnionsCannotBeJsonSerializable = "RMJ0003";
    public const String NoMoreThan31VariantGroupsMayBeDefined = "RMJ0004";
    public const String UnionMayNotBeStatic = "RMJ0005";
    public const String VariantNamesMustBeUnique = "RMJ0006";
    public const String EnsureValidStructUnionState = "RMJ0007";
    public const String InterfaceVariantIsExcludedFromConversionOperators = "RMJ0008";
    public const String VariantTypesMustBeUnique = "RMJ0009";
    public const String ObjectCannotBeUsedAsAVariant = "RMJ0010";
    public const String ValueTypeCannotBeUsedAsAVariantOfStructUnion = "RMJ0019";
    public const String UnionCannotBeUsedAsVariantOfItself = "RMJ0012";
    public const String UnionCannotExplicitlyDefineBaseType = "RMJ0013";
    public const String PreferNullableStructOverIsNullable = "RMJ0014";
    public const String NullableVariantNotAllowedAlongWithNonNullableVariant = "RMJ0015";
    public const String UnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute = "RMJ0016";
    public const String DuplicateVariantGroupNamesAreIgnored = "RMJ0017";
    public const String ClassUnionsShouldBeSealed = "RMJ0018";
    public const String UnionCannotBeRefStruct = "RMJ0020";
#pragma warning restore CS1591
}
