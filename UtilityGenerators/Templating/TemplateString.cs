// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Represents a template string and its position in a C# source text.
/// </summary>
/// <param name="Text">
/// The template string.
/// </param>
/// <param name="Path">
/// The path of the C# file containing the source text, or <see
/// cref="String.Empty"/> if no path can be determined.
/// </param>
/// <param name="Start">
/// The position of the first template character of the template string in the
/// C# source text.
/// </param>
/// <param name="IsMultiline">
/// Indicates whether <see cref="Text"/> is parsed from a multi-line raw string
/// literal. If so, each line may be assumed to on the character column of the
/// first character. Otherwise, the template string originated from a
/// single-line string literal; any newlines encountered may be treated as if on
/// the same line as the starting character.
/// </param>
internal sealed record TemplateString(TemplateSourceText Text, String Path, SourcePosition Start, Boolean IsMultiline)
{
    /// <summary>
    /// Creates a new template string from the syntax token provided. The token
    /// is assumed to represent a string literal value.
    /// </summary>
    /// <param name="token">
    /// The token to create a template string from.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request template string creation to be
    /// cancelled.
    /// </param>
    /// <returns>
    /// A new template string.
    /// </returns>
    public static TemplateString Create(SyntaxToken token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var location = token.GetLocation();
        // We use source positions for diagnostics currently, but in the future,
        // syntax highlighting mit require we have the unmapped line span
        // position available. This is why we do not use GetMappedLineSpan for
        // now.
        var span = location.GetLineSpan().Span;
        // We assume a string literal token is passed, so the default case
        // extracts the value as if it were a regular string literal token.
        var (start, isMultiline) = token switch
        {
            { RawKind: (Int32)SyntaxKind.MultiLineRawStringLiteralToken } => (GetMultilineStart(token, span, cancellationToken), true),
            { RawKind: (Int32)SyntaxKind.SingleLineRawStringLiteralToken } => (GetSinglelineStart(token, span, cancellationToken), false),
            { RawKind: (Int32)SyntaxKind.StringLiteralToken, Text: ['@', ..] } => (GetVerbatimStart(span, cancellationToken), false),
            _ => (GetRegularStart(span, cancellationToken), false),
        };
        // We leave the actual parsing of the value to roslyn.
        var text = token.ValueText;

        var path = location.SourceTree?.FilePath ?? String.Empty;

        return new(text, path, start, isMultiline);
    }
    private static SourcePosition GetMultilineStart(in SyntaxToken token, in LinePositionSpan span, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Multiline raw string literals require the string to begin on the line
        // following the opening quotes.
        var line = span.Start.Line + 1;

        // The span starts with elided whitespace included, so we add to the
        // starting position character the length of whitespace following the
        // last newline.
        var lastNewlineIndex = token.Text.LastIndexOf('\n');
        var elidedTriviaLength = token.Text.Length - lastNewlineIndex - 1 - GetOpeningQuoteCount(token, cancellationToken);

        var character = elidedTriviaLength;

        return new(line, character);
    }
    private static SourcePosition GetSinglelineStart(in SyntaxToken token, in LinePositionSpan span, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var line = span.Start.Line;
        var character = span.Start.Character;

        // We skip all opening quotes. We ignore dollar signs, as interpolated
        // strings are not allowed for attribute parameters.
        character += GetOpeningQuoteCount(token, cancellationToken);

        return new(line, character);
    }
    private static Int32 GetOpeningQuoteCount(in SyntaxToken token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var i = 0;
        for(; token.Text[i] == '"'; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return i;
    }
    private static SourcePosition GetRegularStart(in LinePositionSpan span, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // We skip the opening quote.
        var result = new SourcePosition(span.Start.Line, span.Start.Character + 1);

        return result;
    }

    private static SourcePosition GetVerbatimStart(in LinePositionSpan span, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // We skip the opening quote, as well as the @ symbol.
        var result = new SourcePosition(span.Start.Line, span.Start.Character + 2);

        return result;
    }
}
