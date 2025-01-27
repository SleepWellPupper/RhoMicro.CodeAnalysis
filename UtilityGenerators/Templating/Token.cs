namespace RhoMicro.CodeAnalysis.Templating;

using System;

/// <summary>
/// Represents a token.
/// </summary>
/// <param name="Kind">
/// The type of token.
/// </param>
/// <param name="TemplateString">
/// The template string from which the token was scanned.
/// </param>
/// <param name="Spans">
/// The spans providing the tokens position in the template string.
/// </param>
internal sealed record Token(TokenKind Kind, TemplateString TemplateString, TokenSpans Spans)
{
    /// <summary>
    /// Gets the tokens lexeme.
    /// </summary>
    public ReadOnlySpan<Char> Lexeme => Spans.TextSpan.Length > 0
        ? TemplateString.Text.AsSpan(Spans.TextSpan.Index, Spans.TextSpan.Length)
        : [];
    /// <summary>
    /// Creates a new token for the <see cref="TokenKind.Eof"/> type.
    /// </summary>
    /// <param name="templateString">
    /// The template string for which to create an eof token.
    /// </param>
    /// <param name="endPosition">
    /// The ending position of the template string, relative to its containing
    /// C# source text.
    /// </param>
    /// <returns>
    /// A new eof token.
    /// </returns>
    public static Token CreateEof(TemplateString templateString, SourcePosition endPosition) => new(
        TokenKind.Eof,
        templateString,
        new(new(templateString.Text.Length, 0), new(endPosition, endPosition)));
    public override String ToString() =>
        $"{Kind} '{Lexeme.ToString().Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")}' (" +
    $"{Spans.SourceSpan.Start.Line},{Spans.SourceSpan.Start.Character},{Spans.SourceSpan.End.Line},{Spans.SourceSpan.End.Character}) " +
        $"({Spans.TextSpan.Index},{Spans.TextSpan.Length})";
}