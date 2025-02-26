namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;

internal sealed partial class SyntaxSpansCalculatingVisitor(CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
{
    private Int32 _index = -1;
    private Int32 _newlineAwareIndex;
    private Int32 _newlineAwareLength;
    private Int32 _length;
    private Int32 _startLine;
    private Int32 _startCharacter;
    private Int32 _endLine;
    private Int32 _endCharacter;

    public TokenSpans GetSpans() =>
        _index == -1
        ? TokenSpans.Empty
        : new TokenSpans(
            TemplateSpan: new(
                index: _index,
                length: _length),
            NewlineAwareTemplateSpan: new(
                index: _newlineAwareIndex,
                length: _newlineAwareLength),
            new SourceSpan(
                start: new SourcePosition(
                    line: _startLine,
                    character: _startCharacter),
                end: new SourcePosition(
                    line: _endLine,
                    character: _endCharacter)));
    protected override void OnToken(Token token)
    {
        if(_index == -1)
        {
            _newlineAwareIndex = token.Spans.NewlineAwareTemplateSpan.Index;
            _index = token.Spans.TemplateSpan.Index;
            _startLine = token.Spans.SourceSpan.Start.Line;
            _startCharacter = token.Spans.SourceSpan.Start.Character;
        }

        _newlineAwareLength += token.Spans.NewlineAwareTemplateSpan.Length;
        _length += token.Spans.TemplateSpan.Length;
        _endLine = token.Spans.SourceSpan.End.Line;
        _endCharacter = token.Spans.SourceSpan.End.Character;
    }
}