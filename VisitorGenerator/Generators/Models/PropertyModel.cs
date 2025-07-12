// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;

internal sealed partial record PropertyModel(
    String Name,
    String Type,
    PropertyFlags Flags)
{
    public static Boolean TryCreate(
        ISymbol propertyCandidate,
        INamedTypeSymbol baseNodeType,
        [NotNullWhen(true)] out PropertyModel? result,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        result = null;

        if(propertyCandidate is not IPropertySymbol property)
            return false;

        if(property is not
            {
                CanBeReferencedByName: true,
                DeclaredAccessibility: Accessibility.Public,
                GetMethod: not null,
                IsStatic: false
            })
        {
            return false;
        }

        var name = property.Name;
        var flags = PropertyFlags.None;

        if(property.Type is { IsReferenceType: true, NullableAnnotation: not NullableAnnotation.NotAnnotated } ||
           property.Type is INamedTypeSymbol { } namedPropertyType &&
           namedPropertyType.ConstructedFrom.SpecialType is SpecialType.System_Nullable_T)
        {
            flags |= PropertyFlags.IsNullable;
        }

        var propertyType =
            property.Type is INamedTypeSymbol t && t.Inherits(baseNodeType)
            ? t
            : null;

        if(propertyType is null)
        {
            var @interface = property.Type as INamedTypeSymbol;

            for(var i = 0; i < property.Type.AllInterfaces.Length; i++)
            {
                if(@interface is { ConstructedFrom.SpecialType: SpecialType.System_Collections_Generic_IEnumerable_T } &&
                    @interface.TypeArguments is [INamedTypeSymbol arg] &&
                    arg.Inherits(baseNodeType))
                {
                    propertyType = arg;
                    flags |= PropertyFlags.IsEnumerable;
                    break;
                }

                @interface = property.Type.AllInterfaces[i];
            }
        }

        if(propertyType is null)
            return false;

        var type = propertyType.ToDisplayString();

        result = new PropertyModel(name, type, flags);

        return true;
    }
}
