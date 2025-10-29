// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;

/// <summary>
/// Detects leading whitespace as interpolation indentation.
/// </summary>
/// <remarks>
/// For example, given the following interpolated string:
/// <code>
/// $"  foo{"\nbar\nbaz"}"
/// </code>
/// , the builder using this detector strategy will build the following string
/// (lines are surrounded with single quotes for clarity and not emitted by the builder):
/// <code>
/// '  foo'
/// '  bar'
/// '  baz'
/// </code>
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal sealed class WhitespaceInterpolationIndentationDetector : IInterpolationIndentationDetector
{
    private WhitespaceInterpolationIndentationDetector()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static WhitespaceInterpolationIndentationDetector Instance { get; } = new();

    /// <inheritdoc />
    public Boolean TryDetectIndentation(
            ReadOnlyMemory<Char> text,
            InterpolationIndentationDetectorContext context,
            out ReadOnlyMemory<Char> indentation)
    {
        var whitespaceCount = 0;
        while (text.Span.Length > whitespaceCount && Char.IsWhiteSpace(text.Span[whitespaceCount]))
        {
            whitespaceCount++;
        }

        if (whitespaceCount is 0)
        {
            indentation = default;
            return false;
        }

        indentation = text[..whitespaceCount];
        return true;
    }
}
