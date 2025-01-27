namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;

/// <summary>
/// Provides static indentation values.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class Indentations
{
    public static ReadOnlySpan<Char> Space => " ".AsSpan();
    public static ReadOnlySpan<Char> Space2 => "  ".AsSpan();
    public static ReadOnlySpan<Char> Space4 => "    ".AsSpan();
    public static ReadOnlySpan<Char> Tab => "\t".AsSpan();
    public static ReadOnlySpan<Char> SingleLineComment => "// ".AsSpan();
    public static ReadOnlySpan<Char> DocumentationComment => "/// ".AsSpan();
}
