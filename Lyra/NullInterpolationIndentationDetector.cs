// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;

/// <summary>
/// Implements an empty interpolation indentation detector that will never detect indentations. 
/// </summary>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal sealed class NullInterpolationIndentationDetector : IInterpolationIndentationDetector
{
    private NullInterpolationIndentationDetector()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static NullInterpolationIndentationDetector Instance { get; } = new();

    /// <summary>
    /// Never detects indentation.
    /// </summary>
    /// <param name="indentation">
    /// Upon returning, will always contain <see langword="default"/>.
    /// </param>
    /// <param name="text">
    /// <inheritdoc />
    /// </param>
    /// <param name="context">
    /// <inheritdoc />
    /// </param>
    /// <returns>
    /// <see langword="false"/>
    /// </returns>
    public Boolean TryDetectIndentation(
            ReadOnlyMemory<Char> text,
            InterpolationIndentationDetectorContext context,
            out ReadOnlyMemory<Char> indentation)
    {
        indentation = default;
        return false;
    }
}
