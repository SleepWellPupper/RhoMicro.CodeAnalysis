// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// Represents an attribute syntax.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
/// <param name="Type">
/// The type of the attribute to append.
/// </param>
/// <param name="Arguments">
/// The arguments to append into the attribute.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct AttributeComponent(
        TypeNameComponent Type,
        ImmutableArray<AttributeArgumentComponent> Arguments)
        : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Arguments.IsDefault)
        {
            return;
        }

        builder.Append($"[{Type}");

        if (Arguments is not [])
        {
            builder.Append('(');

            for (var i = 0; i < Arguments.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (i is not 0)
                {
                    builder.Append(", ");
                }

                var argument = Arguments[i];
                builder.Append(argument);
            }

            builder.Append(')');
        }

        builder.Append(']');
    }

    /// <inheritdoc />
    public Boolean Equals(AttributeComponent other)
    {
        if (other.Type != Type)
        {
            return false;
        }

        if (!ImmutableArrayEqualityComparer.Equals(other.Arguments, Arguments))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(Type);
        hc.Add(Arguments, ImmutableArrayEqualityComparer<AttributeArgumentComponent>.Default);
        var result = hc.ToHashCode();

        return result;
    }
}
