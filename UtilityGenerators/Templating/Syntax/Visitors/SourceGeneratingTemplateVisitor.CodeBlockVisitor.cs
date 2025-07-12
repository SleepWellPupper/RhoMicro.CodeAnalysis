// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;

internal sealed partial class SourceGeneratingTemplateVisitor
{
    private sealed class CodeBlockVisitor(SourceGeneratingTemplateVisitor parent, CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
    {
        public override void Visit(CloseCodeBlockSyntax syntax) { }
        public override void Visit(OpenCodeBlockSyntax syntax) { }
        public override void Visit(EscapeColonSyntax syntax) { }
        public override void Visit(NewlineSyntax syntax) { }
        public override void Visit(WhitespacesSyntax syntax) { }
        public override void Visit(RenderBlockSyntax syntax) => syntax.Accept(parent);
        protected override void OnToken(Token token)
        {
            if(token.TemplateString.Path is not { Length: > 0 } path)
                return;

            var detentCount = 0;
            for(; detentCount < parent._builder.OpenBlocks; detentCount++)
                parent._builder.DetentCore();

            parent._builder
                .Append("#line (")
                .Append(token.Spans.SourceSpan.Start.Line + 1)
                .Append(", ")
                .Append(token.Spans.SourceSpan.Start.Character + 1)
                .Append(") - (")
                .Append(token.Spans.SourceSpan.End.Line + 1)
                .Append(", ")
                .Append(token.Spans.SourceSpan.End.Character + 1)
                .AppendCore(')');

            parent._builder.Append(" \"").Append(path).AppendCore('"');

            parent._builder
                .AppendLine()
                .AppendLine(token.Lexeme.ToString())
                .Append("#line default")
                .AppendLineCore();

            for(; detentCount > 0; detentCount--)
                parent._builder.IndentCore();
        }
    }
}
