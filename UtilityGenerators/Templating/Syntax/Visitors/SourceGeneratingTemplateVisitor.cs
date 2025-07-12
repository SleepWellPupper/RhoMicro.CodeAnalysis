// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;

using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating.Syntax;

internal sealed partial class SourceGeneratingTemplateVisitor : TreeWalkingSyntaxVisitor
{
    public SourceGeneratingTemplateVisitor(
        IndentedStringBuilder builder,
        TemplateAttribute.Model attribute,
        TemplateString templateString,
        CancellationToken ct) : base(ct)
    {
        _builder = builder;
        _templateString = templateString;
        _codeBlockVisitor = new(this, ct);
        _templateBlockVisitor = new(this, ct);
        _attribute = attribute;
    }

    private readonly TemplateAttribute.Model _attribute;
    private readonly IndentedStringBuilder _builder;
    private readonly TemplateString _templateString;
    private readonly CodeBlockVisitor _codeBlockVisitor;

    private readonly TemplateBlockVisitor _templateBlockVisitor;

    private Int32 _fragmentCount = -1;

    public override void Visit(CodeBlockSyntax syntax) => syntax.Accept(_codeBlockVisitor);
    public override void Visit(RenderBlockSyntax syntax)
    {
        syntax.RenderBlockBody?.Accept(this);

        var headSpans = syntax.RenderBlockHead.Text.GetSyntaxSpans(Ct);

        _builder
            .Append(_attribute.RendererParameterName)
            .Append(".Render(")
            .AppendLineCore();

        var detentCount = 0;
        for(; detentCount < _builder.OpenBlocks; detentCount++)
            _builder.DetentCore();

        _builder
            .Append("#line (")
            .Append(headSpans.SourceSpan.Start.Line + 1)
            .Append(", ")
            .Append(headSpans.SourceSpan.Start.Character + 1)
            .Append(") - (")
            .Append(headSpans.SourceSpan.End.Line + 1)
            .Append(", ")
            .Append(headSpans.SourceSpan.End.Character + 1)
            .AppendCore(')');

        if(_templateString.Path is { Length: > 0 } path)
            _builder.Append(" \"").Append(path).AppendCore('"');

        _builder
            .AppendLine()
            .AppendLine(_templateString
                .Text
                .ToString()
                .Substring(
                    headSpans.TemplateSpan.Index,
                    headSpans.TemplateSpan.Length))
            .Append("#line default")
            .AppendLineCore();

        for(; detentCount > 0; detentCount--)
            _builder.IndentCore();

        if(syntax.RenderBlockBody is { })
        {
            _builder
                .Append(", ")
                .Append(_attribute.FragmentName)
                .Append('_')
                .AppendCore(_fragmentCount);
        }

        _builder
            .Append(");")
            .AppendLineCore();
    }
    public override void Visit(NotEmptyTemplateSyntax syntax)
    {
        syntax.Accept(_templateBlockVisitor);
        _templateBlockVisitor.Flush();
    }
    public override void Visit(TemplateBlockSyntax syntax)
    {
        _builder
            .Append("void ")
            .Append(_attribute.FragmentName)
            .Append('_')
            .Append(++_fragmentCount)
            .Append("(ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer ")
            .Append(_attribute.RendererParameterName)
            .Append(", global::System.Threading.CancellationToken ")
            .Append(_attribute.CancellationTokenParameterName)
            .Append(')')
            .OpenBracesBlock()
            .Append(_attribute.CancellationTokenParameterName)
            .Append(".ThrowIfCancellationRequested();")
            .AppendLineCore();

        syntax.TemplateBlockBody.Accept(_templateBlockVisitor);
        _templateBlockVisitor.Flush();

        _builder
            .CloseBlockCore();
    }
}