namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using RhoMicro.CodeAnalysis.Templating.Syntax;

internal interface ISyntaxVisitor
{
    public void Visit(CloseBlockSyntax syntax);
    public void Visit(CloseCodeBlockSyntax syntax);
    public void Visit(CloseRenderBlockSyntax syntax);
    public void Visit(CloseTemplateBlockSyntax syntax);
    public void Visit(CodeBlockBodyChildSyntax syntax);
    public void Visit(CodeBlockBodySyntax syntax);
    public void Visit(CodeBlockSyntax syntax);
    public void Visit(EmptyBlockSyntax syntax);
    public void Visit(EmptyTemplateSyntax syntax);
    public void Visit(EscapeColonSyntax syntax);
    public void Visit(EscapedCloseBlockSyntax syntax);
    public void Visit(EscapedOpenBlockSyntax syntax);
    public void Visit(EscapedTextSyntax syntax);
    public void Visit(LeadingTriviaSyntax syntax);
    public void Visit(NewlineSyntax syntax);
    public void Visit(NotEmptyTemplateSyntax syntax);
    public void Visit(NotEscapedTextChildSyntax syntax);
    public void Visit(NotEscapedTextSyntax syntax);
    public void Visit(NotNewlineSyntax syntax);
    public void Visit(OpenBlockSyntax syntax);
    public void Visit(OpenCodeBlockSyntax syntax);
    public void Visit(OpenRenderBlockSyntax syntax);
    public void Visit(OpenTemplateBlockSyntax syntax);
    public void Visit(RenderBlockBodySyntax syntax);
    public void Visit(RenderBlockHeadSyntax syntax);
    public void Visit(RenderBlockSyntax syntax);
    public void Visit(RenderBlockTriviaSyntax syntax);
    public void Visit(TemplateBlockBodyChildSyntax syntax);
    public void Visit(TemplateBlockBodySyntax syntax);
    public void Visit(TemplateBlockSyntax syntax);
    public void Visit(TemplateSyntax syntax);
    public void Visit(TextChildSyntax syntax);
    public void Visit(TextSyntax syntax);
    public void Visit(TrailingTriviaSyntax syntax);
    public void Visit(WhitespacesSyntax syntax);
}
