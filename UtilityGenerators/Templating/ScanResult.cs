// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

/// <summary>
/// Represents the result of scanning a template string for tokens.
/// </summary>
/// <param name="Tokens">
/// The tokens scanned.
/// </param>
/// <param name="TemplateString">
/// The template string that was scanned to obtain the scan result.
/// </param>
/// <param name="RequiredQuotes">
/// The amount of quotes required to syntactically correctly enclose the
/// template string in a raw string literal.
/// </param>
/// <param name="Diagnostics">
/// The diagnostics accumulated while scanning.
/// </param>
internal sealed record ScanResult(
    EquatableList<Token> Tokens,
    TemplateString TemplateString,
    Int32 RequiredQuotes,
    EquatableList<Diagnostic> Diagnostics);
