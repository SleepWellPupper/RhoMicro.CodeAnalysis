// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// Represents a type declaration.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
/// <param name="Modifiers">
/// The modifiers to prepend in front of the name.
/// </param>
/// <param name="Name">
/// The name of the type.
/// </param>
/// <param name="TypeParameters">
/// The types generic type parameters.
/// </param>
/// <param name="BaseTypeList">
/// The list of types to append into the base type list.
/// </param>
/// <param name="Attributes">
/// The list of attributes to prepend to the type declaration.
/// </param>
/// <param name="Body">
/// The body of the type.
/// </param>
/// <typeparam name="TBody">
/// The type of the body.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct TypeComponent<TBody>(
        String Modifiers,
        String Name,
        ImmutableArray<String> TypeParameters,
        ImmutableArray<TypeNameComponent> BaseTypeList,
        ImmutableArray<AttributeComponent> Attributes,
        TBody Body) : ICSharpSourceComponent
        where TBody : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Name is null)
        {
            return;
        }

        if (Attributes is not [])
        {
            foreach (var attribute in Attributes)
            {
                builder.AppendLine(attribute);
            }
        }

        if (Modifiers is not [])
        {
            builder.Append($"{Modifiers} ");
        }

        builder.Append($"{Name}");

        if (TypeParameters is not [])
        {
            builder.Append('<');

            for (var i = 0; i < TypeParameters.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (i > 0)
                {
                    builder.Append(", ");
                }

                var parameter = TypeParameters[i];
                builder.Append(parameter);
            }

            builder.Append('>');
        }

        if (BaseTypeList is not [])
        {
            builder.Append(" : ");

            for (var i = 0; i < BaseTypeList.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (i > 0)
                {
                    builder.Append(", ");
                }

                var baseType = BaseTypeList[i];
                builder.Append(baseType);
            }
        }

        builder.AppendLine()
               .AppendLine('{')
               .Indent()
               .AppendLine(Body)
               .Detent()
               .Append('}');
    }

    /// <inheritdoc />
    public Boolean Equals(TypeComponent<TBody> other)
    {
        if (other.Modifiers != Modifiers)
        {
            return false;
        }

        if (other.Modifiers != Modifiers)
        {
            return false;
        }

        if (other.Name != Name)
        {
            return false;
        }

        if (!EqualityComparer<TBody>.Default.Equals(other.Body, Body))
        {
            return false;
        }

        if (!ImmutableArrayEqualityComparer.Equals(other.TypeParameters, TypeParameters))
        {
            return false;
        }

        if (!ImmutableArrayEqualityComparer.Equals(other.BaseTypeList, BaseTypeList))
        {
            return false;
        }

        if (!ImmutableArrayEqualityComparer.Equals(other.Attributes, Attributes))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Modifiers);
        hc.Add(Modifiers);
        hc.Add(Name);
        hc.Add(Body, EqualityComparer<TBody>.Default);
        hc.Add(TypeParameters, ImmutableArrayEqualityComparer<String>.Default);
        hc.Add(BaseTypeList, ImmutableArrayEqualityComparer<TypeNameComponent>.Default);
        hc.Add(Attributes, ImmutableArrayEqualityComparer<AttributeComponent>.Default);

        var result = hc.ToHashCode();

        return result;
    }
}
