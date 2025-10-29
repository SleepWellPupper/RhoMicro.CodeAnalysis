// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;

/// <summary>
/// Detects leading comments and whitespace as interpolation indentation.
/// </summary>
/// <remarks>
/// For example, given the following interpolated string:
/// <code>
/// $"//  foo{"\nbar\nbaz"}"
/// </code>
/// , the builder using this detector strategy will build the following string:
/// <code>
/// //  foo
/// //  bar
/// //  baz
/// </code>
/// The detector supports docs comments (<c>///</c>) and single line comment (<c>//</c>).
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal sealed class CommentInterpolationIndentationDetector : IInterpolationIndentationDetector
{
    private CommentInterpolationIndentationDetector()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CommentInterpolationIndentationDetector Instance { get; } = new();

    /// <inheritdoc/>
    public Boolean TryDetectIndentation(
            ReadOnlyMemory<Char> text,
            InterpolationIndentationDetectorContext context,
            out ReadOnlyMemory<Char> indentation)
    {
        if (text.Span is not ['/', '/', ..])
        {
            indentation = default;
            return false;
        }

        var whitespaceCount = text.Span is ['/', '/', '/', ..] ? 3 : 2;
        while (text.Span.Length > whitespaceCount && Char.IsWhiteSpace(text.Span[whitespaceCount]))
        {
            whitespaceCount++;
        }

        indentation = text[..whitespaceCount];
        return true;
    }
}
