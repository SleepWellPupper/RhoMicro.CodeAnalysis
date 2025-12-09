// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a union type.
/// </summary>
public interface IUnion
{
    /// <summary>
    /// Maps the variant value represented by this instance to a union of type <typeparamref name="TUnion"/>.
    /// </summary>
    /// <param name="factory">
    /// The factory to use when creating an instance of <typeparamref name="TUnion"/>.
    /// </param>
    /// <typeparam name="TUnion">
    /// The type of union to map to.
    /// </typeparam>
    /// <typeparam name="TFactory">
    /// The type of factory to use when creating an instance of <typeparamref name="TUnion"/>.
    /// </typeparam>
    /// <returns>
    /// A new instance of <typeparamref name="TUnion"/> representing the value represented by this instance.
    /// </returns>
    TUnion MapTo<TUnion, TFactory>(TFactory factory)
        where TFactory : IUnionFactory<TUnion>;


    /// <summary>
    /// Attempts to map the variant value represented by this instance to a union of type <typeparamref name="TUnion"/>.
    /// </summary>
    /// <param name="factory">
    /// The factory to use when attempting to create an instance of <typeparamref name="TUnion"/>.
    /// </param>
    /// <param name="union">
    /// A new instance of <typeparamref name="TUnion"/> representing the value represented by this instance,
    /// if the factory indicates success; otherwise, <see langword="default"/>.
    /// </param>
    /// <typeparam name="TUnion">
    /// The type of union to attempt to map to.
    /// </typeparam>
    /// <typeparam name="TFactory">
    /// The type of factory to use when attempting to create an instance of <typeparamref name="TUnion"/>.
    /// </typeparam>
    /// <returns>
    /// <see langword="this"/> if the factory indicates success; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryMapTo<TUnion, TFactory>(TFactory factory, [NotNullWhen(true)] out TUnion? union)
        where TFactory : IUnionFactory<TUnion>;
}
