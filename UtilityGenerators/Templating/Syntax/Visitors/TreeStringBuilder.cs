namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

[NonEquatable]
internal abstract partial class TreeStringBuilder<TSelf>(CancellationToken ct)
    : ISyntaxVisitor
    where TSelf : TreeStringBuilder<TSelf>
{
    protected IndentedStringBuilder Builder { get; } = new(
        IndentedStringBuilderOptions.Default with
        {
            PrependMarkerComment = false,
            AmbientCancellationToken = ct,
            DefaultIndentation = ' ',
            NewLine = '\n'
        });
    protected CancellationToken Ct { get; } = ct;

    public override String ToString() => Builder.ToString();

    protected void Append<TChildSyntax>(String production, EquatableList<TChildSyntax> children, Boolean appendNewLine = true)
        where TChildSyntax : ISyntax
    {
        Ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, children) =>
            {
                @this.Ct.ThrowIfCancellationRequested();

                foreach(var child in children)
                {
                    @this.Ct.ThrowIfCancellationRequested();
                    child.Accept(@this);
                }
            },
            children,
            appendNewLine);
    }
    protected void Append(String production, ISyntax? child1 = null, ISyntax? child2 = null, ISyntax? child3 = null, ISyntax? child4 = null, ISyntax? child5 = null, Boolean appendNewLine = true)
    {
        Ct.ThrowIfCancellationRequested();

        Append(
            production,
            static (@this, children) =>
            {
                @this.Ct.ThrowIfCancellationRequested();

                children[0]?.Accept(@this);
                children[1]?.Accept(@this);
                children[2]?.Accept(@this);
                children[3]?.Accept(@this);
                children[4]?.Accept(@this);
            },
            new[] { child1, child2, child3, child4, child5 },
            appendNewLine);
    }

    protected abstract void Append(String production, Token token);
    protected abstract void Append<TState>(String production, Action<TSelf, TState> body, TState state, Boolean appendNewLine = true);
    protected abstract void Append(String production, Boolean appendNewLine = true);

    public void Visit(CodeBlockSyntax syntax) => Append(CodeBlockSyntax.Production, syntax.LeadingTrivia, syntax.OpenCodeBlock, syntax.CodeBody, syntax.CloseCodeBlock, syntax.TrailingTrivia);
    public void Visit(CodeBlockBodySyntax syntax) => Append(CodeBlockBodySyntax.Production, syntax.Children);
    public void Visit(CodeBlockBodyChildSyntax syntax) => Append(CodeBlockBodyChildSyntax.Production, syntax.Child);
    public void Visit(TemplateBlockSyntax syntax) => Append(TemplateBlockSyntax.Production, syntax.LeadingTrivia, syntax.OpenTemplateBlock, syntax.TemplateBlockBody, syntax.CloseTemplateBlock, syntax.TrailingTrivia);
    public void Visit(TextSyntax syntax) => Append(TextSyntax.Production, syntax.Children);
    public void Visit(NotEscapedTextSyntax syntax) => Append(NotEscapedTextSyntax.Production, syntax.Children);
    public void Visit(RenderBlockSyntax syntax) => Append(RenderBlockSyntax.Production, syntax.RenderBlockHead, syntax.RenderBlockBody);
    public void Visit(RenderBlockHeadSyntax syntax) => Append(RenderBlockHeadSyntax.Production, syntax.OpenRenderBlock, syntax.Text, syntax.CloseRenderBlock);
    public void Visit(RenderBlockBodySyntax syntax) => Append(RenderBlockBodySyntax.Production, syntax.RenderBlockTrivia, syntax.TemplateBlock);
    public void Visit(EscapedOpenBlockSyntax syntax) => Append(EscapedOpenBlockSyntax.Production, syntax.OpenBlock, syntax.EscapeColon);
    public void Visit(EscapedCloseBlockSyntax syntax) => Append(EscapedCloseBlockSyntax.Production, syntax.EscapeColon, syntax.CloseBlock);
    public void Visit(EscapeColonSyntax syntax) => Append(EscapeColonSyntax.Production, syntax.Token);
    public void Visit(OpenRenderBlockSyntax syntax) => Append(OpenRenderBlockSyntax.Production, syntax.Token);
    public void Visit(OpenCodeBlockSyntax syntax) => Append(OpenCodeBlockSyntax.Production, syntax.Token);
    public void Visit(OpenTemplateBlockSyntax syntax) => Append(OpenTemplateBlockSyntax.Production, syntax.Token);
    public void Visit(CloseRenderBlockSyntax syntax) => Append(CloseRenderBlockSyntax.Production, syntax.Token);
    public void Visit(CloseCodeBlockSyntax syntax) => Append(CloseCodeBlockSyntax.Production, syntax.Token);
    public void Visit(CloseTemplateBlockSyntax syntax) => Append(CloseTemplateBlockSyntax.Production, syntax.Token);
    public void Visit(EmptyBlockSyntax syntax) => Append(EmptyBlockSyntax.Production, syntax.OpenBlock, syntax.CloseBlock);
    public void Visit(TemplateBlockBodySyntax syntax) => Append(TemplateBlockBodySyntax.Production, syntax.Children);
    public void Visit(NotEmptyTemplateSyntax syntax) => Append(NotEmptyTemplateSyntax.Production, syntax.TemplateBlockBody, appendNewLine: false);
    public void Visit(EmptyTemplateSyntax syntax) => Append(EmptyTemplateSyntax.Production, appendNewLine: false);
    public void Visit(CloseBlockSyntax syntax) => Append(CloseBlockSyntax.Production, syntax.Child);
    public void Visit(EscapedTextSyntax syntax) => Append(EscapedTextSyntax.Production, syntax.Child);
    public void Visit(LeadingTriviaSyntax syntax) => Append(LeadingTriviaSyntax.Production, syntax.Whitespaces);
    public void Visit(NewlineSyntax syntax) => Append(NewlineSyntax.Production, syntax.Token);
    public void Visit(NotEscapedTextChildSyntax syntax) => Append(NotEscapedTextChildSyntax.Production, syntax.Child);
    public void Visit(NotNewlineSyntax syntax) => Append(NotNewlineSyntax.Production, syntax.Token);
    public void Visit(OpenBlockSyntax syntax) => Append(OpenBlockSyntax.Production, syntax.Child);
    public void Visit(RenderBlockTriviaSyntax syntax) => Append(RenderBlockTriviaSyntax.Production, syntax.Newline);
    public void Visit(TemplateBlockBodyChildSyntax syntax) => Append(TemplateBlockBodyChildSyntax.Production, syntax.Child);
    public void Visit(TemplateSyntax syntax) => Append(TemplateSyntax.Production, syntax.Child);
    public void Visit(TextChildSyntax syntax) => Append(TextChildSyntax.Production, syntax.Child);
    public void Visit(TrailingTriviaSyntax syntax) => Append(TrailingTriviaSyntax.Production, syntax.Newline);
    public void Visit(WhitespacesSyntax syntax) => Append(WhitespacesSyntax.Production, syntax.Token);
}
