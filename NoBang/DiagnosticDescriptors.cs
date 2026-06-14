// SPDX-License-Identifier: MPL-2.0

using Microsoft.CodeAnalysis;

namespace NoBang;

/// <summary>
/// Shared diagnostic descriptors for the analyzer.
/// </summary>
public static class DiagnosticDescriptors
{
    /// <summary>
    /// Reports use of the null forgiving operator.
    /// </summary>
    public static DiagnosticDescriptor TheNullForgivingOperatorIsACodeSmell { get; }
        = new(
            id: "RMNB0001",
            title: "The null forgiving operator is a code smell and should be avoided",
            messageFormat: "Avoid using the null forgiving operator.",
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
}
