namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal sealed class XmlTreeStringBuilder(CancellationToken ct) : TreeStringBuilder<XmlTreeStringBuilder>(ct)
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
                    .Append(t.Kind.ToString())
                    .Append(" position=\"")
                    .Append(t.Spans.ToString())
                    .Append("\">")
                    .Append(t.Lexeme.ToString().Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t"))
                    .Append("</t:")
                    .Append(t.Kind.ToString())
                    .Append('>')
                    .AppendLineCore();
            },
            token);
    }
    protected override void Append<TState>(String production, Action<XmlTreeStringBuilder, TState> body, TState state, Boolean appendNewLine = true)
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
