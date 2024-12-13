namespace RhoMicro.CodeAnalysis.Library.Models;
using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

[IncludeFile]
internal readonly record struct ContainingTypeSignatureModel(PartialTypeKindModel Kind, String Name, IList<String> TypeArguments)
{
    public static ContainingTypeSignatureModel Create(INamedTypeSymbol containingType, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var name = containingType.Name;
        var typeArguments = ctx.CollectionFactory.CreateList<String>();

        foreach(var typeArgument in containingType.TypeArguments)
        {
            ctx.ThrowIfCancellationRequested();

            typeArguments.Add(typeArgument.Name);
        }

        var kind = PartialTypeKindModel.Create(containingType, in ctx);

        var result = new ContainingTypeSignatureModel(kind, name, typeArguments);

        return result;
    }
}