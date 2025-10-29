// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// Renders a separated list of elements to a <see cref="CSharpSourceBuilder"/>.
/// </summary>
/// <remarks>
/// This type supports value equality. The <see cref="Append"/> and <see cref="Separate"/>
/// members are not taken into account when calculating equality. 
/// </remarks>
/// <param name="Elements">
/// The elements to append to the builder.
/// </param>
/// <param name="Append">
/// The callback invoked to append elements to the builder.
/// </param>
/// <param name="Separator">
/// The separator separating each element.
/// </param>
/// <param name="Separate">
/// The callback invoked to append a separator to the builder.
/// </param>
/// <param name="Terminator">
/// The terminator to append to the final element.
/// </param>
/// <typeparam name="TElement">
/// The type of element to append.
/// </typeparam>
/// <typeparam name="TSeparator">
/// The type of separator to append.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct ListComponent<TElement, TSeparator>(
        ImmutableArray<TElement> Elements,
        Action<TElement, Int32, Int32, CSharpSourceBuilder, CancellationToken> Append,
        TSeparator Separator,
        Action<TSeparator, Int32, Int32, CSharpSourceBuilder, CancellationToken> Separate,
        TSeparator Terminator)
        : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Elements.IsDefault)
        {
            return;
        }

        var length = Elements.Length;

        for (var index = 0; index < length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (index is not 0)
            {
                Separate.Invoke(Separator, index, length, builder, cancellationToken);
            }

            var element = Elements[index];
            Append.Invoke(element, index, length, builder, cancellationToken);

            if (index == length - 1)
            {
                Separate.Invoke(Terminator, index, length, builder, cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public Boolean Equals(ListComponent<TElement, TSeparator> other)
    {
        if (!EqualityComparer<TSeparator>.Default.Equals(other.Separator, Separator))
        {
            return false;
        }

        if (!EqualityComparer<TSeparator>.Default.Equals(other.Terminator, Terminator))
        {
            return false;
        }

        if (!ImmutableArrayEqualityComparer.Equals(other.Elements, Elements))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(Separator);
        hc.Add(Terminator);
        hc.Add(Elements, ImmutableArrayEqualityComparer<TElement>.Default);
        var result = hc.ToHashCode();

        return result;
    }
}
