// SPDX-License-Identifier: MPL-2.0

using System.Diagnostics.CodeAnalysis;

public interface IUnion
{
    TUnion MapTo<TUnion, TFactory>(TFactory factory)
            where TFactory : IUnionFactory<TUnion>;

    bool TryMapTo<TUnion, TFactory>(TFactory factory, [NotNullWhen(true)] out TUnion? union)
            where TFactory : IUnionFactory<TUnion>;
}
