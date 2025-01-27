namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;

[NonEquatable]
internal sealed partial class TokenCounter(CancellationToken ct) : ISyntaxVisitor
{
    public Int32 Count { get; private set; }

    public void Visit(TextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public void Visit(NotEscapedTextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(BlockSequenceSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Block.Accept(this);

        foreach(var child in syntax.Blocks)
        {
            ct.ThrowIfCancellationRequested();
            child.Accept(this);
        }
    }
    public void Visit(TriviaBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Trivia.Accept(this);
        syntax.Block.Accept(this);
    }
    public void Visit(TriviaSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(RenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.RenderBlockHead.Accept(this);
        if(syntax.RenderBlockBody is { } body)
            body.Accept(this);
    }
    public void Visit(RenderBlockHeadSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenRenderBlock.Accept(this);
        syntax.Text.Accept(this);
        syntax.CloseRenderBlock.Accept(this);
    }
    public void Visit(RenderBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.TemplateBlock.Accept(this);
    }
    public void Visit(TemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenTemplateBlock.Accept(this);
        syntax.TemplateBlockBody.Accept(this);
        syntax.CloseTemplateBlock.Accept(this);
    }
    public void Visit(CodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenCodeBlock.Accept(this);
        syntax.CodeBody.Accept(this);
        syntax.CloseCodeBlock.Accept(this);
    }
    public void Visit(CodeBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();
            child.Accept(this);
        }
    }
    public void Visit(CodeBodyChildSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(syntax.TryAsRenderBlock(out var renderBlock))
            renderBlock.Accept(this);
        else if(syntax.TryAsText(out var text))
            text.Accept(this);
    }
    public void Visit(EmptyBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.CloseBlock.Accept(this);
    }
    public void Visit(EscapedOpenBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.EscapeColon.Accept(this);
    }
    public void Visit(EscapedCloseBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Colon.Accept(this);
        syntax.CloseBlock.Accept(this);
    }
    public void Visit(EscapeColonSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(OpenRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(OpenCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(OpenTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(CloseRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(CloseCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(CloseTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        Count++;
    }
    public void Visit(TemplateBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();
            child.Accept(this);
        }
    }

    public void Visit(NotEmptyTemplateSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.TemplateBlockBody.Accept(this);
    }

    public void Visit(EmptyTemplateSyntax syntax) => ct.ThrowIfCancellationRequested();
}
