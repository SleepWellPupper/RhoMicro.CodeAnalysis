// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Reflection;

using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal sealed partial class TokenCountingVisitor(CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
{
    public Int32 Count { get; private set; }
    protected override void OnToken(Token token)
    {
        Ct.ThrowIfCancellationRequested();

        Count++;
    }
}
