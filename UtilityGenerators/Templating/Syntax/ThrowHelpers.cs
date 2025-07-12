// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;
using System;
using System.Runtime.CompilerServices;
using System.Text;

using RhoMicro.CodeAnalysis.Templating;

internal static class ThrowHelpers
{
    public static void ThrowIfKindNotEqual(Token token, TokenKind kind, [CallerArgumentExpression(nameof(token))] String? parameterName = null)
    {
        if(token.Kind != kind)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"{parameterName} must be of kind {kind}, but was {token.Kind}.");
        }
    }
}
