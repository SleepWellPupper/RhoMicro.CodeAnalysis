// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

public interface IUnionFactory<TUnion>
{
    TUnion Create<TVariant>(TVariant value);
    bool TryCreate<TVariant>(TVariant value, [NotNullWhen(true)] out TUnion? union);
}
