// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Creates instances of <typeparamref name="TUnion"/>.
/// </summary>
/// <typeparam name="TUnion">
/// The type of union to create.
/// </typeparam>
public interface IUnionFactory<TUnion>
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="TUnion"/> from a variant value.
    /// </summary>
    /// <param name="value">
    /// The value to create an instance of <typeparamref name="TUnion"/> from.
    /// </param>
    /// <typeparam name="TVariant">
    /// The type of variant to create an instance of <typeparamref name="TUnion"/> from.
    /// </typeparam>
    /// <returns>
    /// A new instance of <typeparamref name="TUnion"/>, containing <paramref name="value"/>.
    /// </returns>
    TUnion Create<TVariant>(TVariant value);

    /// <summary>
    /// Attempts to create a new instance of <typeparamref name="TUnion"/> from a variant value.
    /// </summary>
    /// <param name="value">
    /// The value to attempt to create an instance of <typeparamref name="TUnion"/> from.
    /// </param>
    /// <param name="union">
    /// The created instance of <typeparamref name="TUnion"/> if <typeparamref name="TUnion"/>
    /// can represent <paramref name="value"/>; otherwise, <see langword="default"/>.
    /// </param>
    /// <typeparam name="TVariant">
    /// The type of the variant to create an instance of <typeparamref name="TUnion"/> from.
    /// </typeparam>
    /// <returns>
    /// <see langword="true"/> if an instance of <typeparamref name="TUnion"/> could be created;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool TryCreate<TVariant>(TVariant value, [NotNullWhen(true)] out TUnion? union);
}
