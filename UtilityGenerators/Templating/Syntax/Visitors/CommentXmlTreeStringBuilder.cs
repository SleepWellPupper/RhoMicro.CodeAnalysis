// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Templating;

internal sealed class CommentXmlTreeStringBuilder(CancellationToken ct) : CommentTreeStringBuilder<CommentXmlTreeStringBuilder>(ct)
{
    protected override void Append(String production, Token token)
    {
        Ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {

                @this.Ct.ThrowIfCancellationRequested();

                @this.Builder
                    .Append("<t:")
                    .Append(t.kind.ToString())
                    .Append('>')
                    .Append(t.kind is TokenKind.Newline ? t.lexeme.Replace("\r", "\\r").Replace("\n", "\\n") : t.lexeme.Replace("\t", "\\t"))
                    .Append("</t:")
                    .Append(t.kind.ToString())
                    .Append('>')
                    .AppendLineCore();
            },
            (kind: token.Kind, lexeme: token.Lexeme.ToString()));
    }
    protected override void Append<TState>(String production, Action<CommentXmlTreeStringBuilder, TState> body, TState state, Boolean appendNewLine = true)
    {
        Ct.ThrowIfCancellationRequested();

        Builder
            .Append("<s:")
            .Append(production)
            .AppendLine('>')
            .IndentCore();

        body.Invoke(this, state);

        Builder
            .Detent()
            .Append("</s:")
            .Append(production)
            .AppendCore('>');

        if(appendNewLine)
            Builder.AppendLineCore();
    }
    protected override void Append(String production, Boolean appendNewLine = true)
    {
        Ct.ThrowIfCancellationRequested();

        Builder
            .Append("<s:")
            .Append(production)
            .Append("/>")
            .AppendLineCore();

        if(appendNewLine)
            Builder.AppendLineCore();
    }
}
