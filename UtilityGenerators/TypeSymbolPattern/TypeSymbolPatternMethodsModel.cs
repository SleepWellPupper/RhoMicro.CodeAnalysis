// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using System.Collections.Immutable;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record TypeSymbolPatternMethodsModel(NamedTypeModel ContainingType, EquatableList<TypeSymbolPatternMethodModel> Methods)
{
    public static EquatableList<TypeSymbolPatternMethodsModel> CreateAll(ImmutableArray<TypeSymbolPatternMethodsPartialModel> models, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var map = new Dictionary<NamedTypeModel, EquatableList<TypeSymbolPatternMethodModel>>();
        var result = ctx.CollectionFactory.CreateList<TypeSymbolPatternMethodsModel>();

        foreach(var (containingType, method) in models)
        {
            if(!map.TryGetValue(containingType, out var methods))
            {
                methods = ctx.CollectionFactory.CreateList<TypeSymbolPatternMethodModel>();
                map.Add(containingType, methods);
                result.Add(new(containingType, methods));
            }

            methods.Add(method);
        }

        return result;
    }
}
