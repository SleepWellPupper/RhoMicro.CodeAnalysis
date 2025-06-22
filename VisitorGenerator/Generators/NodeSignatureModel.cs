namespace RhoMicro.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record NodeSignatureModel(
    String Namespace,
    NodeSignatureFlags Flags,
    String Name,
    EquatableList<String> TypeParameters)
{
    public static NodeSignatureModel Create(ITypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var @namespace = type.ContainingNamespace?.ToDisplayString(SymbolDisplayFormats.NamespaceFormat) ?? String.Empty;
        var name = type.Name;

        var flags = NodeSignatureFlags.None;

        if(type.IsRecord)
            flags |= NodeSignatureFlags.IsRecord;

        if(type.DeclaredAccessibility is Accessibility.Public)
            flags |= NodeSignatureFlags.IsPublic;

        var typeParameters = ctx.CollectionFactory.CreateList<String>();

        if(type is INamedTypeSymbol namedType)
        {
            foreach(var param in namedType.TypeParameters)
            {
                ctx.ThrowIfCancellationRequested();

                typeParameters.Add(param.Name);
            }
        }

        var result = new NodeSignatureModel(@namespace, flags, name, typeParameters);

        return result;
    }
}
