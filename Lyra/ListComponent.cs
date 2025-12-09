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
/// <param name="List">
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
/// <typeparam name="TList">
/// The type of list to render.
/// </typeparam>
/// <typeparam name="TElement">
/// The type of element to append.
/// </typeparam>
/// <typeparam name="TSeparator">
/// The type of separator to append.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct ListComponent<TList, TElement, TSeparator>(
    TList List,
    Action<TElement, Int32, Int32, CSharpSourceBuilder, CancellationToken> Append,
    TSeparator Separator,
    Action<TSeparator, Int32, Int32, CSharpSourceBuilder, CancellationToken> Separate,
    TSeparator Terminator)
    : ICSharpSourceComponent
    where TList : IList<TElement>
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var length = List.Count;

        for (var index = 0; index < length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (index is not 0)
            {
                Separate.Invoke(Separator, index, length, builder, cancellationToken);
            }

            var element = List[index];
            Append.Invoke(element, index, length, builder, cancellationToken);

            if (index == length - 1)
            {
                Separate.Invoke(Terminator, index, length, builder, cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public Boolean Equals(ListComponent<TList, TElement, TSeparator> other)
    {
        if (!EqualityComparer<TSeparator>.Default.Equals(other.Separator, Separator))
        {
            return false;
        }

        if (!EqualityComparer<TSeparator>.Default.Equals(other.Terminator, Terminator))
        {
            return false;
        }

        if (!other.List.SequenceEqual(List))
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

        foreach (var element in List)
        {
            hc.Add(element);
        }

        var result = hc.ToHashCode();

        return result;
    }
}
