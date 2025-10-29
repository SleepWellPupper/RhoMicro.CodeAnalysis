// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Immutable;

/// <summary>
/// Aggregates several indentation detectors into a chain of responsibility.
/// </summary>
/// <param name="detectors">
/// The detectors to aggregate.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal sealed class AggregateInterpolationIndentationDetector(
        params ImmutableArray<IInterpolationIndentationDetector> detectors) : IInterpolationIndentationDetector
{
    /// <summary>
    /// Attempts to detect indentation using all the registered detectors,
    /// returning upon the first successful detection.
    /// </summary>
    /// <inheritdoc/>
    public Boolean TryDetectIndentation(
            ReadOnlyMemory<Char> text,
            InterpolationIndentationDetectorContext context,
            out ReadOnlyMemory<Char> indentation)
    {
        foreach (var detector in detectors)
        {
            if (!detector.TryDetectIndentation(text, context, out var i))
            {
                continue;
            }

            indentation = i;
            return true;
        }

        indentation = default;
        return false;
    }
}
