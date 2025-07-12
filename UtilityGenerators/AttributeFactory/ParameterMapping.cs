// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

internal sealed record ParameterMapping(Int32 ConstructorIndex, Int32 ParameterIndex, String ParameterName, String PropertyName)
{
    public override String ToString() => $"{{{ConstructorIndex}-{ParameterIndex}: {ParameterName}->{PropertyName}}}";
}
