namespace RhoMicro.CodeAnalysis.Templating;

using RhoMicro.CodeAnalysis.Library;

using System;

/// <summary>
/// Represents a span of characters in a template string.
/// </summary>
internal readonly record struct TemplateSpan
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="index">
    /// The index (0-based) of the span in the template string.
    /// </param>
    /// <param name="length">
    /// The length of the span.
    /// </param>
    public TemplateSpan(Int32 index, Int32 length)
    {
        ThrowHelpers.ArgumentOutOfRangeException.ThrowIfNegative(index);
        ThrowHelpers.ArgumentOutOfRangeException.ThrowIfNegative(length);

        Index = index;
        Length = length;
    }
    /// <summary>
    /// Gets the index (0-based) of the span in the template string.
    /// </summary>
    public Int32 Index { get; }
    /// <summary>
    /// Gets the length of the span.
    /// </summary>
    public Int32 Length { get; }

    public override String ToString() => $"({Index},{Length})";
}
