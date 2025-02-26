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
internal readonly record struct Token(TokenKind Kind, TemplateString TemplateString, TokenSpans Spans)
{
    /// <summary>
    /// Gets the tokens lexeme from the <see cref="TokenSpans.TemplateSpan"/>.
    /// </summary>
    public ReadOnlySpan<Char> Lexeme => Spans.TemplateSpan.Length > 0
        ? TemplateString.Text.ToString().AsSpan(Spans.TemplateSpan.Index, Spans.TemplateSpan.Length)
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
    /// <param name="newlineOffset">
    /// The difference in length between the original template string and the
    /// newline aware translation.
    /// </param>
    /// <returns>
    /// A new eof token.
    /// </returns>
    public static Token CreateEof(TemplateString templateString, SourcePosition endPosition, Int32 newlineOffset) => new(
        TokenKind.Eof,
        templateString,
        new(
            TemplateSpan: new(templateString.Text.ToString().Length, 0),
            NewlineAwareTemplateSpan: new(templateString.Text.ToString().Length + newlineOffset, 0),
            new(endPosition, endPosition)));
    public override String ToString() =>
        $"{Kind} '{Lexeme.ToString().Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")}' {Spans}";
}