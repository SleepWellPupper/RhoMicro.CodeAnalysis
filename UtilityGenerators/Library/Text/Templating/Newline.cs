// SPDX-License-Identifier: MPL-2.0

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
    Lf,
    /// <summary>
    /// Represents the <c>\r</c> newline.
    /// </summary>
    Cr,
    /// <summary>
    /// Represents the <c>\r\n</c> newline.
    /// </summary>
    CrLf
}
