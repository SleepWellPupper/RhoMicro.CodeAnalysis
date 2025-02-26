namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;

/// <summary>
/// Provides static newline values.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal enum Newline
{
    /// <summary>
    /// Represents the <c>\n</c> newline.
    /// </summary>
    Newline,
    /// <summary>
    /// Represents the <c>\r</c> newline.
    /// </summary>
    CarriageReturn,
    /// <summary>
    /// Represents the <c>\r\n</c> newline.
    /// </summary>
    CarriageReturnNewline
}