namespace RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Diagnostic IDs start with ROG")]
internal static class DiagnosticIds
{
    public const String ROG0001OptionInterfacesCannotBeGeneric = "ROG0001";
    public const String ROG0002OptionsPropertiesMustBeReadOnly = "ROG0002";
    public const String ROG0003OptionInterfacesCannotBeNested = "ROG0003";
}