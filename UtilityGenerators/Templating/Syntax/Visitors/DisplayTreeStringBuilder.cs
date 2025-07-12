// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal sealed class DisplayTreeStringBuilder(CancellationToken ct) : TreeStringBuilder<DisplayTreeStringBuilder>(ct)
{
    protected override void Append(String production, Token token)
    {
        Ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, token) =>
            {
                @this.Ct.ThrowIfCancellationRequested();

                @this.Builder.AppendCore(token.Lexeme.ToString());
            },
            token);
    }
    protected override void Append<TState>(String production, Action<DisplayTreeStringBuilder, TState> body, TState state, Boolean appendNewLine = true)
    {
        Ct.ThrowIfCancellationRequested();

        body.Invoke(this, state);
    }
    protected override void Append(String production, Boolean appendNewLine = true) => Ct.ThrowIfCancellationRequested();
}
