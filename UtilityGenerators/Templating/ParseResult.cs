// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using System.Linq;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax;

/// <summary>
/// Represents the result of parsing a template string.
/// </summary>
/// <param name="Syntax">
/// The template syntax parsed.
/// </param>
/// <param name="ScanResult">
/// The scan result that was parsed to obtain the parse result.
/// </param>
/// <param name="Diagnostics">
/// The diagnostics accumulated while parsing.
/// </param>
internal sealed record ParseResult(
    TemplateSyntax Syntax,
    ScanResult ScanResult,
    EquatableList<Diagnostic> Diagnostics)
{
    /// <summary>
    /// Enumerates first combined diagnostics from this parse result, and the
    /// scan result used to obtain it.
    /// </summary>
    public IEnumerable<Diagnostic> AllDiagnostics => ScanResult.Diagnostics.Concat(Diagnostics);
}
