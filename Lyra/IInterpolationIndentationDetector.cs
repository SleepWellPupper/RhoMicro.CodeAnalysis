// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;

/// <summary>
/// Used to detect indentations for interpolated strings passed to <see cref="CSharpSourceBuilder"/>.
/// </summary>
/// <remarks>
/// For example, consider this interpolated string:
/// <code>
/// $"//  foo{bar}"
/// </code>
/// It is possible that <c>bar</c> evaluates to a string containing linebreaks.
/// This is not immediately obvious from the interpolated string itself.
/// Therefore, this interface provides the ability to detect and use the comment
/// preceding it for further indentation.
/// In this particular case, the <see cref="CommentInterpolationIndentationDetector"/>
/// will recognize the '<c>//  </c>' preceding '<c>foo</c>' and utilize it for
/// indentation on all lines that <c>bar</c> evaluates to.
/// Assuming that <c>bar</c> evaluates to <c>\nbaz\nfaz</c>, the following output will be built as a result:
/// <code>
/// //  foo
/// //  baz
/// //  faz
/// </code>
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal interface IInterpolationIndentationDetector
{
    /// <summary>
    /// Attempts to detect indentation from the provided text.
    /// </summary>
    /// <param name="text">
    /// The text to scan for indentation.
    /// </param>
    /// <param name="context">
    /// A context object providing access to buffers and builder options.
    /// Buffers may be obtained from this object in order to create artificial
    /// indentation.
    /// </param>
    /// <param name="indentation">
    /// Upon returning from the method, contains the indentation if one could be
    /// detected; otherwise, <see langword="default"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if indentation could be determined; otherwise, <see langword="false"/>.
    /// </returns>
    Boolean TryDetectIndentation(
            ReadOnlyMemory<Char> text,
            InterpolationIndentationDetectorContext context,
            out ReadOnlyMemory<Char> indentation);
}
