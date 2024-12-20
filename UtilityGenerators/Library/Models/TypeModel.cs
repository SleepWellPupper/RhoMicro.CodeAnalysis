namespace RhoMicro.CodeAnalysis.Library.Models;
using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal record TypeModel(
    EquatableList<String> NamespaceParts,
    String Name)
{
    public static TypeModel Create(ITypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(type is INamedTypeSymbol named)
            return NamedTypeModel.Create(named, in ctx);

        if(type is IArrayTypeSymbol array)
            return ArrayTypeModel.Create(array, in ctx);

        var namespaceParts = GetNamespaceParts(type, in ctx);
        var name = type.Name;

        var result = new TypeModel(
            namespaceParts,
            name);

        return result;
    }
    protected static EquatableList<String> GetNamespaceParts(ITypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var result = ctx.CollectionFactory.CreateList<String>();

        AddNamespaceParts(type.ContainingNamespace, result, in ctx);

        return result;
    }
    private static void AddNamespaceParts(INamespaceSymbol? @namespace, EquatableList<String> parts, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(@namespace is null or { IsGlobalNamespace: true })
            return;

        AddNamespaceParts(@namespace.ContainingNamespace, parts, in ctx);

        parts.Add(@namespace.Name);
    }
}
