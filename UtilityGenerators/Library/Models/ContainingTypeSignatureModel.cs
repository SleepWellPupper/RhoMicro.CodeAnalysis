// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models;
using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if RHOMICRO_EMIT_PUBLIC_COLLECTIONS
public
#else
internal 
#endif
readonly record struct ContainingTypeSignatureModel(
    PartialTypeKindModel Kind,
    String Name,
    EquatableList<TypeModel> TypeArguments)
{
    public static ContainingTypeSignatureModel Create(INamedTypeSymbol containingType, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var name = containingType.Name;
        var typeArguments = ctx.CollectionFactory.CreateList<TypeModel>();

        foreach(var typeArgument in containingType.TypeArguments)
        {
            ctx.ThrowIfCancellationRequested();
            var model = TypeModel.Create(typeArgument, in ctx);
            typeArguments.Add(model);
        }

        var kind = PartialTypeKindModel.Create(containingType, in ctx);

        var result = new ContainingTypeSignatureModel(kind, name, typeArguments);

        return result;
    }
}
