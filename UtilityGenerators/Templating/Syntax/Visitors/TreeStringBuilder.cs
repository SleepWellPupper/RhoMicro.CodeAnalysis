namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

[NonEquatable]
internal sealed partial class TreeStringBuilder(CancellationToken ct = default) : ISyntaxVisitor
{
    private readonly IndentedStringBuilder _builder = new(
        IndentedStringBuilderOptions.Default with
        {
            PrependMarkerComment = false,
            AmbientCancellationToken = ct,
            DefaultIndentation = ' ',
            NewLine = '\n'
        });

    public override String ToString() => _builder.ToString();

    private void Append<TChildSyntax>(String production, EquatableList<TChildSyntax> children, Boolean appendNewLine = true)
        where TChildSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {
                t.ct.ThrowIfCancellationRequested();

                foreach(var child in t.children)
                {
                    t.ct.ThrowIfCancellationRequested();
                    child.Accept(@this);
                }
            },
            (ct, children),
            appendNewLine);
    }
    private void Append<TChildSyntax>(String production, TChildSyntax child, Boolean appendNewLine = true)
        where TChildSyntax : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {
                t.ct.ThrowIfCancellationRequested();

                t.child.Accept(@this);
            },
            (ct, child),
            appendNewLine);
    }
    private void Append<TChildSyntax1, TChildSyntax2>(String production, TChildSyntax1 child1, TChildSyntax2 child2)
        where TChildSyntax1 : ISyntax
        where TChildSyntax2 : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {
                t.ct.ThrowIfCancellationRequested();

                t.child1.Accept(@this);
                t.child2.Accept(@this);
            },
            (ct, child1, child2));
    }
    private void Append<TChildSyntax1, TChildSyntax2, TChildSyntax3>(String production, TChildSyntax1 child1, TChildSyntax2 child2, TChildSyntax3 child3)
        where TChildSyntax1 : ISyntax
        where TChildSyntax2 : ISyntax
        where TChildSyntax3 : ISyntax
    {
        ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {
                t.ct.ThrowIfCancellationRequested();

                t.child1.Accept(@this);
                t.child2.Accept(@this);
                t.child3.Accept(@this);
            },
            (ct, child1, child2, child3));
    }
    private void Append(String production, Token token)
    {
        ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, t) =>
            {
                t.ct.ThrowIfCancellationRequested();

                @this._builder
                    .Append("<t:")
                    .Append(t.type.ToString())
                    .Append('>')
                    .Append(t.lexeme.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t"))
                    .Append("</t:")
                    .Append(t.type.ToString())
                    .Append('>')
                    .AppendLineCore();
            },
            (ct, type: token.Kind, lexeme: token.Lexeme.ToString()));
    }
    private void Append<TState>(String production, Action<TreeStringBuilder, TState> body, TState state, Boolean appendNewLine = true)
    {
        ct.ThrowIfCancellationRequested();

        _builder
            .Append("<s:")
            .Append(production)
            .AppendLine('>')
            .IndentCore();

        body.Invoke(this, state);

        _builder
            .Detent()
            .Append("</s:")
            .Append(production)
            .AppendCore('>');

        if(appendNewLine)
            _builder.AppendLineCore();
    }
    private void Append(String production, Boolean appendNewLine = true)
    {
        ct.ThrowIfCancellationRequested();

        _builder
            .Append("<s:")
            .Append(production)
            .Append("/>")
            .AppendLineCore();

        if(appendNewLine)
            _builder.AppendLineCore();
    }

    public void Visit(CodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(CodeBlockSyntax.Production, syntax.OpenCodeBlock, syntax.CodeBody, syntax.CloseCodeBlock);
    }
    public void Visit(CodeBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(CodeBodySyntax.Production, syntax.Children);
    }
    public void Visit(CodeBodyChildSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(syntax.TryAsText(out var text))
            text.Accept(this);
        else if(syntax.TryAsRenderBlock(out var renderBlock))
            renderBlock.Accept(this);
    }
    public void Visit(TemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(TemplateBlockSyntax.Production, syntax.OpenTemplateBlock, syntax.TemplateBlockBody, syntax.CloseTemplateBlock);
    }
    public void Visit(TextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(TextSyntax.Production, syntax.Children);
    }
    public void Visit(NotEscapedTextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(NotEscapedTextSyntax.Production, syntax.Token);
    }
    public void Visit(RenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(syntax.RenderBlockBody is { } body)
            Append(RenderBlockSyntax.Production, syntax.RenderBlockHead, body);
        else
            Append(RenderBlockSyntax.Production, syntax.RenderBlockHead);
    }
    public void Visit(RenderBlockHeadSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(RenderBlockHeadSyntax.Production, syntax.OpenRenderBlock, syntax.Text, syntax.CloseRenderBlock);
    }
    public void Visit(RenderBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(syntax.Trivia is { } trivia)
            Append(RenderBlockBodySyntax.Production, trivia, syntax.TemplateBlock);
        else
            Append(RenderBlockBodySyntax.Production, syntax.TemplateBlock);
    }
    public void Visit(EscapedOpenBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(
        EscapedOpenBlockSyntax.Production,
        static (@this, syntax) =>
        {
            syntax.OpenBlock.Accept(@this);
            syntax.EscapeColon.Accept(@this);
        },
        syntax);
    }
    public void Visit(EscapedCloseBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(
        EscapedCloseBlockSyntax.Production,
        static (@this, syntax) =>
        {
            syntax.Colon.Accept(@this);
            syntax.CloseBlock.Accept(@this);
        },
        syntax);
    }
    public void Visit(EscapeColonSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(EscapeColonSyntax.Production, syntax.Token);
    }
    public void Visit(OpenRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(OpenRenderBlockSyntax.Production, syntax.Token);
    }
    public void Visit(OpenCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(OpenCodeBlockSyntax.Production, syntax.Token);
    }
    public void Visit(OpenTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(OpenTemplateBlockSyntax.Production, syntax.Token);
    }
    public void Visit(CloseRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(CloseRenderBlockSyntax.Production, syntax.Token);
    }
    public void Visit(CloseCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(CloseCodeBlockSyntax.Production, syntax.Token);
    }
    public void Visit(CloseTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(CloseTemplateBlockSyntax.Production, syntax.Token);
    }
    public void Visit(TriviaSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(TriviaSyntax.Production, syntax.Token);
    }
    public void Visit(BlockSequenceSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(
        BlockSequenceSyntax.Production,
        static (@this, t) =>
        {
            t.ct.ThrowIfCancellationRequested();

            t.syntax.Block.Accept(@this);
            foreach(var child in t.syntax.Blocks)
            {
                t.ct.ThrowIfCancellationRequested();

                child.Accept(@this);
            }
        },
        (ct, syntax));
    }
    public void Visit(TriviaBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(TriviaBlockSyntax.Production, syntax.Trivia, syntax.Block);
    }
    public void Visit(EmptyBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(EmptyBlockSyntax.Production, syntax.OpenBlock, syntax.CloseBlock);
    }
    public void Visit(TemplateBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(TemplateBlockBodySyntax.Production, syntax.Children);
    }
    public void Visit(NotEmptyTemplateSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(NotEmptyTemplateSyntax.Production, syntax.TemplateBlockBody);
    }
    public void Visit(EmptyTemplateSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Append(EmptyTemplateSyntax.Production);
    }
}
