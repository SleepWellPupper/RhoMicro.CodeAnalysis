// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using System.Runtime.CompilerServices;

/// <summary>
/// Represents the a template diagnostic.
/// </summary>
/// <param name="SourceSpan">
/// The source span the diagnostic was emitted for.
/// </param>
/// <param name="Id">
/// The id of the diagnostic.
/// </param>
/// <param name="Message">
/// The message of the diagnostic.
/// </param>
/// <param name="Severity">
/// The severity of the diagnostic.
/// </param>
/// <param name="Path">
/// The file path the diagnostic was emitted for.
/// </param>
internal sealed record Diagnostic(String Id, String Message, DiagnosticSeverity Severity, String Path, SourceSpan SourceSpan)
{
    public static class Ids
    {
        public const String UnexpectedToken = "RMT0001";
    }

    private static readonly Dictionary<String, String> _messages = new()
    {
        [Ids.UnexpectedToken] = "Unexpected token.",
    };

    public static Diagnostic Create(String id, DiagnosticSeverity severity, String path, SourceSpan sourceSpan) =>
        new(id, _messages.TryGetValue(id, out var message) ? message : ThrowIdOutOfRange(id), severity, path, sourceSpan);

    private static String ThrowIdOutOfRange(
        String id,
        [CallerArgumentExpression(nameof(id))] String? parameterName = null)
        => throw new ArgumentOutOfRangeException(parameterName, id, $"{parameterName} '{id}' is not a known diagnostic id.");

    public override String ToString() => $"{Path}{SourceSpan}: {Severity.ToLowerString()} {Id}: {Message}";
}

file static class Extensions
{
    public static String ToLowerString(this DiagnosticSeverity severity) => severity switch
    {
        DiagnosticSeverity.Info => "info",
        DiagnosticSeverity.Warning => "warning",
        DiagnosticSeverity.Error => "error",
        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, "Unknown severity.")
    };
}