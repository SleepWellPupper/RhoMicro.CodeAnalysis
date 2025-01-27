namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal interface ISyntax
{
    void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor;
}
