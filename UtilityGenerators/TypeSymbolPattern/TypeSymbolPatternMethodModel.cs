namespace RhoMicro.CodeAnalysis;
using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
internal sealed record TypeSymbolPatternMethodModel(Accessibility Accessibility, Boolean IsStatic, String MethodName, TypeSymbolPatternAttributeModel Type)
{
    public static TypeSymbolPatternMethodModel Create(IMethodSymbol target, AttributeData attribute, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var accessibility = target.DeclaredAccessibility;
        var isStatic = target.IsStatic;
        var methodName = target.Name;
        var type = TypeSymbolPatternAttributeModel.Create(attribute, in ctx);

        var result = new TypeSymbolPatternMethodModel(
            accessibility,
            isStatic,
            methodName,
            type);

        return result;
    }
}
