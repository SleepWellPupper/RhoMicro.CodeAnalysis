namespace RhoMicro.CodeAnalysis.Templating;
using System;

using RhoMicro.CodeAnalysis.Library;

/// <summary>
/// Represents a position in the C# source text from which a template was parsed.
/// </summary>
internal readonly record struct SourcePosition : IComparable<SourcePosition>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="line">
    /// The line index (0-based) in the C# source text.
    /// </param>
    /// <param name="character">
    /// The character index (0-based) on the line in the C# source text.
    /// </param>
    public SourcePosition(Int32 line, Int32 character)
    {
        ThrowHelpers.ArgumentOutOfRangeException.ThrowIfNegative(line);
        ThrowHelpers.ArgumentOutOfRangeException.ThrowIfNegative(character);

        Line = line;
        Character = character;
    }

    /// <summary>
    /// Gets an empty source position.
    /// </summary>
    public static SourcePosition Empty { get; } = default;

    /// <summary>
    /// Gets the (0,0) source position marking the beginning of a source text.
    /// </summary>
    public static SourcePosition Start { get; } = default;

    /// <summary>
    /// Gets the line index (0-based) in the C# source text.
    /// </summary>
    public Int32 Line { get; }
    /// <summary>
    /// Gets the character index (0-based) on the line in the C# source text.
    /// </summary>
    public Int32 Character { get; }

    public Int32 CompareTo(SourcePosition other)
    {
        if(Line > other.Line)
            return 1;
        if(Line < other.Line)
            return -1;
        if(Character > other.Character)
            return 1;
        if(Character < other.Character)
            return -1;

        return 0;
    }
}