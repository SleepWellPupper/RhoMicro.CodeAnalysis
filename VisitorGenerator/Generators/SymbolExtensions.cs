namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

internal static class SymbolExtensions
{
    public static Boolean Inherits(this INamedTypeSymbol type, INamedTypeSymbol baseType)
        => SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, baseType) ||
        type.ConstructedFrom.BaseType is { } super && super.Inherits(baseType);
}
