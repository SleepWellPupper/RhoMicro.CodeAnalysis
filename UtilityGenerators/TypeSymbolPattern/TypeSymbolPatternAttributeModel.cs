// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;

internal readonly record struct TypeSymbolPatternAttributeModel(Boolean CheckTypeArguments, TypeModel Type)
{
    public static TypeSymbolPatternAttributeModel Create(AttributeData attribute, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var model = attribute.GetTypeSymbolPatternAttributeModel(ctx.CancellationToken);
        var signature = TypeModel.Create(model.Type, in ctx);

        var result = new TypeSymbolPatternAttributeModel(
            model.CheckTypeArguments,
            signature);

        return result;
    }
}
