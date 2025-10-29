// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

/// <summary>
/// Provides static access to equality operations defined by <see cref="ImmutableArrayEqualityComparer{T}.Default"/>.
/// </summary>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal static class ImmutableArrayEqualityComparer
{
    /// <inheritdoc cref="ImmutableArrayEqualityComparer{T}"/>
    public static Boolean Equals<T>(ImmutableArray<T> x, ImmutableArray<T> y) =>
            ImmutableArrayEqualityComparer<T>.Default.Equals(x, y);

    /// <inheritdoc cref="ImmutableArrayEqualityComparer{T}"/>
    public static Int32 GetHashCode<T>(ImmutableArray<T> obj) =>
            ImmutableArrayEqualityComparer<T>.Default.GetHashCode(obj);
}

/// <summary>
/// Implements value equality for <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <param name="elementComparer">
/// The comparer to use for comparing individual elements.
/// </param>
/// <typeparam name="T">
/// The type of elements contained in compared arrays.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal sealed class ImmutableArrayEqualityComparer<T>(IEqualityComparer<T> elementComparer)
        : IEqualityComparer<ImmutableArray<T>>
{
    /// <summary>
    /// Gets an instance using <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    public static ImmutableArrayEqualityComparer<T> Default { get; } = new(EqualityComparer<T>.Default);

    /// <inheritdoc />
    public Boolean Equals(ImmutableArray<T> x, ImmutableArray<T> y)
    {
        if (x.IsDefault)
        {
            return y.IsDefault;
        }

        if (y.IsDefault)
        {
            return false;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (var i = 0; i < x.Length; i++)
        {
            if (!elementComparer.Equals(x[i], y[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public Int32 GetHashCode(ImmutableArray<T> obj)
    {
        var hc = new HashCode();

        foreach (var element in obj)
        {
            hc.Add(element, elementComparer);
        }

        var result = hc.ToHashCode();

        return result;
    }
}
