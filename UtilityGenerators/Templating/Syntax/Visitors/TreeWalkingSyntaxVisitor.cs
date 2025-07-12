// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

[NonEquatable]
internal abstract partial class TreeWalkingSyntaxVisitor(CancellationToken ct) : ISyntaxVisitor
{
    protected virtual void OnToken(Token token) { }

    protected CancellationToken Ct { get; } = ct;

    public virtual void Visit(CloseBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(CloseCodeBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(CloseRenderBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(CloseTemplateBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(CodeBlockBodyChildSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(CodeBlockBodySyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            Ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public virtual void Visit(CodeBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.LeadingTrivia?.Accept(this);
        syntax.OpenCodeBlock.Accept(this);
        syntax.CodeBody.Accept(this);
        syntax.CloseCodeBlock.Accept(this);
        syntax.TrailingTrivia?.Accept(this);
    }
    public virtual void Visit(EmptyBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.CloseBlock.Accept(this);
    }
    public virtual void Visit(EmptyTemplateSyntax syntax) => Ct.ThrowIfCancellationRequested();
    public virtual void Visit(EscapeColonSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(EscapedCloseBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.EscapeColon.Accept(this);
        syntax.CloseBlock.Accept(this);
    }
    public virtual void Visit(EscapedOpenBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.EscapeColon.Accept(this);
    }
    public virtual void Visit(EscapedTextSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(LeadingTriviaSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Whitespaces.Accept(this);
    }
    public virtual void Visit(NewlineSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(NotEmptyTemplateSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.TemplateBlockBody.Accept(this);
    }
    public virtual void Visit(NotEscapedTextChildSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(NotEscapedTextSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            Ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public virtual void Visit(NotNewlineSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(OpenBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(OpenCodeBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(OpenRenderBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(OpenTemplateBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
    public virtual void Visit(RenderBlockBodySyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.RenderBlockTrivia?.Accept(this);
        syntax.TemplateBlock.Accept(this);
    }
    public virtual void Visit(RenderBlockHeadSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.OpenRenderBlock.Accept(this);
        syntax.Text.Accept(this);
        syntax.CloseRenderBlock.Accept(this);
    }
    public virtual void Visit(RenderBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.RenderBlockHead.Accept(this);
        syntax.RenderBlockBody?.Accept(this);
    }
    public virtual void Visit(RenderBlockTriviaSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Newline.Accept(this);
    }
    public virtual void Visit(TemplateBlockBodyChildSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(TemplateBlockBodySyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            Ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public virtual void Visit(TemplateBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.LeadingTrivia?.Accept(this);
        syntax.OpenTemplateBlock.Accept(this);
        syntax.TemplateBlockBody.Accept(this);
        syntax.CloseTemplateBlock.Accept(this);
        syntax.TrailingTrivia?.Accept(this);
    }
    public virtual void Visit(TemplateSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(TextChildSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Child.Accept(this);
    }
    public virtual void Visit(TextSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            Ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public virtual void Visit(TrailingTriviaSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        syntax.Newline.Accept(this);
    }
    public virtual void Visit(WhitespacesSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        OnToken(syntax.Token);
    }
}
