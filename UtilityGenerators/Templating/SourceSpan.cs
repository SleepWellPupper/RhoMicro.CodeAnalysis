namespace RhoMicro.CodeAnalysis.Templating;

using RhoMicro.CodeAnalysis.Library;

/// <summary>
/// Represents a span of characters in the C# source text from which a template was parsed.
/// </summary>
internal readonly record struct SourceSpan
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="start">
    /// The start of the span (inclusive).
    /// </param>
    /// <param name="end">
    /// The end of the span (inclusive).
    /// </param>
    public SourceSpan(SourcePosition start, SourcePosition end)
    {
        ThrowHelpers.ArgumentOutOfRangeException.ThrowIfLessThan(end, start);

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets an empty source span.
    /// </summary>
    public static SourceSpan Empty { get; } = default;

    /// <summary>
    /// Gets the start of the span (inclusive).
    /// </summary>
    public SourcePosition Start { get; }
    /// <summary>
    /// Gets the end of the span (inclusive).
    /// </summary>
    public SourcePosition End { get; }

    public override String ToString() => $"({Start.Line},{Start.Character},{End.Line},{End.Character})";
}