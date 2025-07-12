// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using Microsoft.CodeAnalysis;

internal static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat NamespaceFormat { get; }
        = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);
}
