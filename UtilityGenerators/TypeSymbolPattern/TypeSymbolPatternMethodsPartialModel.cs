namespace RhoMicro.CodeAnalysis;
using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;

internal sealed record TypeSymbolPatternMethodsPartialModel(NamedTypeModel ContainingType, TypeSymbolPatternMethodModel Method)
{
    public static TypeSymbolPatternMethodsPartialModel Create(IMethodSymbol target, AttributeData attribute, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var containingType = NamedTypeModel.Create(target.ContainingType, in ctx);
        var method = TypeSymbolPatternMethodModel.Create(target, attribute, in ctx);

        var result = new TypeSymbolPatternMethodsPartialModel(
            containingType,
            method);

        return result;
    }
}
