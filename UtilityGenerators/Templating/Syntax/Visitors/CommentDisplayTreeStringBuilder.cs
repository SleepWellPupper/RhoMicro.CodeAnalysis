// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Library.Text.Templating;
using RhoMicro.CodeAnalysis.Templating;

internal sealed class CommentDisplayTreeStringBuilder(CancellationToken ct) : CommentTreeStringBuilder<CommentDisplayTreeStringBuilder>(ct)
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
    protected override void Append<TState>(String production, Action<CommentDisplayTreeStringBuilder, TState> body, TState state, Boolean appendNewLine = true)
    {
        Ct.ThrowIfCancellationRequested();

        body.Invoke(this, state);
    }
    protected override void Append(String production, Boolean appendNewLine = true) => Ct.ThrowIfCancellationRequested();
}
