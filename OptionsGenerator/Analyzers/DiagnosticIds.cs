namespace RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Diagnostic IDs start with ROG")]
internal static class DiagnosticIds
{
    public const String ROG0001TargetInterfacesCannotBeGeneric = "ROG0001";
    public const String ROG0002OptionsPropertiesMustBeReadOnly = "ROG0002";
}