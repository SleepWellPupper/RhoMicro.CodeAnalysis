// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

/// <summary>
/// Represents the type of a template token.
/// </summary>
internal enum TokenKind
{
    OpenCodeBlock,
    CloseCodeBlock,

    OpenRenderBlock,
    CloseRenderBlock,

    OpenTemplateBlock,
    CloseTemplateBlock,

    Whitespaces,
    Newline,
    NotNewline,

    EscapeColon,

    /// <summary>
    /// The end of file token. This token is defined to terminate any template
    /// tokenization. The eof is defined to always start after the last
    /// character (on the same line), and to be of length 0.
    /// </summary>
    Eof
}
