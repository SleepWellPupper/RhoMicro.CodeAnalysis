namespace RhoMicro.CodeAnalysis.OptionsGenerator.GeneratorsV2;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat GlobalOmittedNamespaceFormat { get; } = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
}
