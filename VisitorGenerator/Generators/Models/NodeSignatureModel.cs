// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record NodeSignatureModel(
    String Namespace,
    NodeSignatureFlags Flags,
    String Name,
    EquatableList<String> TypeParameters)
{
    public static Boolean TryCreate(ITypeSymbol type, [NotNullWhen(true)] out NodeSignatureModel? result, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if (!type.IsSealed && !type.IsAbstract)
        {
            result = null;
            return false;
        }

        var @namespace = type.ContainingNamespace?.ToDisplayString(SymbolDisplayFormats.NamespaceFormat) ?? String.Empty;
        var name = type.Name;

        var flags = NodeSignatureFlags.None;

        if (type.IsRecord)
            flags |= NodeSignatureFlags.IsRecord;

        if (type.DeclaredAccessibility is Accessibility.Public)
            flags |= NodeSignatureFlags.IsPublic;

        if (type.IsSealed)
            flags |= NodeSignatureFlags.IsSealed;

        var typeParameters = ctx.CollectionFactory.CreateList<String>();

        if (type is INamedTypeSymbol namedType)
        {
            foreach (var param in namedType.ConstructedFrom.TypeParameters)
            {
                ctx.ThrowIfCancellationRequested();

                typeParameters.Add(param.Name);
            }
        }

        result = new NodeSignatureModel(@namespace, flags, name, typeParameters);
        return true;
    }
}
