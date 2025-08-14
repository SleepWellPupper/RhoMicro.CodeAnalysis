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
sealed record ArrayTypeModel(
    TypeModel ElementType,
    EquatableList<String> NamespaceParts,
    String Name) : TypeModel(
        NamespaceParts,
        Name)
{
    public static ArrayTypeModel Create(IArrayTypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var name = type.Name;
        var namespaceParts = GetNamespaceParts(type, in ctx);
        var elementType = Create(type.ElementType, in ctx);

        var result = new ArrayTypeModel(
            elementType,
            namespaceParts,
            name);

        return result;
    }
}
