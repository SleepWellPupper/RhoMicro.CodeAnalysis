// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Buffers;
using System.Collections.Generic;

/// <summary>
/// Manages the lifetime of arrays rented from a <see cref="ArrayPool{T}"/>.
/// Arrays may be rented from the lifetime and are returned upon disposal of the lifetime.
/// </summary>
/// <param name="pool">
/// The pool whose arrays to manage lifetimes for.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly struct RentedArrayLifetime(ArrayPool<Char> pool) : IDisposable
{
    private readonly List<Char[]> _rentedArrays = [];

    /// <summary>
    /// Gets the pool whose rented arrays lifetimes this instance manages.
    /// </summary>
    public ArrayPool<Char> Pool => pool;

    /// <summary>
    /// Rents an array from <see cref="Pool"/>.
    /// The array is returned to the pool upon disposal of this instance.
    /// </summary>
    /// <param name="minimumLength">
    /// <inheritdoc cref="ArrayPool{T}.Rent"/>
    /// </param>
    /// <returns>
    /// An array rented from <see cref="Pool"/>, whose lifetime is managed by this instance.
    /// </returns>
    public Char[] Rent(Int32 minimumLength)
    {
        var result = Pool.Rent(minimumLength);
        _rentedArrays.Add(result);
        return result;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var rentedBuffer in _rentedArrays)
        {
            Pool.Return(rentedBuffer);
        }

        _rentedArrays.Clear();
    }
}
