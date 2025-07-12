// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

public partial class ParserTests
{
    private sealed class TokenList(EquatableList<Token> tokens)
    {
        public EquatableList<Token> Tokens { get; } = tokens;
        private Int32 _index;

        public Token Next() => Tokens[_index++];
    }
}
