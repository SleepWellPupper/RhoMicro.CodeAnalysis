namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;
internal interface ISyntaxVisitor
{
    void Visit(NotEmptyTemplateSyntax syntax);
    void Visit(EmptyTemplateSyntax syntax);
    
    void Visit(TextSyntax syntax);
    void Visit(NotEscapedTextSyntax syntax);

    void Visit(TemplateBlockBodySyntax syntax);

    void Visit(BlockSequenceSyntax syntax);
    void Visit(TriviaBlockSyntax syntax);
    void Visit(TriviaSyntax syntax);

    void Visit(RenderBlockSyntax syntax);
    void Visit(RenderBlockHeadSyntax syntax);
    void Visit(RenderBlockBodySyntax syntax);

    void Visit(TemplateBlockSyntax syntax);

    void Visit(CodeBlockSyntax syntax);
    void Visit(CodeBodySyntax syntax);
    void Visit(CodeBodyChildSyntax syntax);

    void Visit(EmptyBlockSyntax syntax);

    void Visit(EscapedOpenBlockSyntax syntax);
    void Visit(EscapedCloseBlockSyntax syntax);
    void Visit(EscapeColonSyntax syntax);

    void Visit(OpenRenderBlockSyntax syntax);
    void Visit(OpenCodeBlockSyntax syntax);
    void Visit(OpenTemplateBlockSyntax syntax);

    void Visit(CloseRenderBlockSyntax syntax);
    void Visit(CloseCodeBlockSyntax syntax);
    void Visit(CloseTemplateBlockSyntax syntax);
}
