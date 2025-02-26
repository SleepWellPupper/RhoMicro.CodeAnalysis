namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;

internal sealed partial class SourceGeneratingTemplateVisitor
{
    private sealed class TemplateBlockVisitor(SourceGeneratingTemplateVisitor parent, CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
    {
        private Int32 _index = -1;
        private Int32 _length = 0;

        public override void Visit(RenderBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
            syntax.Accept(parent);
        }
        public override void Visit(CodeBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
            syntax.Accept(parent);
        }
        public override void Visit(TemplateBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
            syntax.Accept(parent);
        }

        public void Flush()
        {
            Ct.ThrowIfCancellationRequested();

            if(_index == -1)
                return;

            if(_length != 0)
            {
                parent._builder
                    .Append(parent._attribute.RendererParameterName)
                    .Append(".Render(")
                    .Append(parent._attribute.TemplateConstName)
                    .Append(".AsSpan(")
                    .Append(_index)
                    .Append(", ")
                    .Append(_length)
                    .Append("));")
                    .AppendLineCore();
            }

            _index = -1;
            _length = 0;
        }

        public override void Visit(EmptyBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            var spans = syntax.GetSyntaxSpans(Ct);
            OnSpans(spans);
        }
        public override void Visit(EscapedCloseBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            var spans = syntax.CloseBlock.GetSyntaxSpans(Ct);
            OnSpans(spans);
        }
        public override void Visit(EscapedOpenBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            var spans = syntax.OpenBlock.GetSyntaxSpans(Ct);
            OnSpans(spans);
        }
        public override void Visit(EscapeColonSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
        }
        public override void Visit(CloseTemplateBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
        }
        public override void Visit(OpenTemplateBlockSyntax syntax)
        {
            Ct.ThrowIfCancellationRequested();

            Flush();
        }
        private void OnSpans(TokenSpans spans)
        {
            Ct.ThrowIfCancellationRequested();

            if(_index == -1)
                _index = spans.NewlineAwareTemplateSpan.Index;

            _length += spans.NewlineAwareTemplateSpan.Length;
        }
        protected override void OnToken(Token token)
        {
            Ct.ThrowIfCancellationRequested();

            OnSpans(token.Spans);
        }
    }
}