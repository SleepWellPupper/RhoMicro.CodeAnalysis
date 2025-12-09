// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Provides options for appending type names.
/// </summary>
/// <param name="UseTypeAliases">
/// Indicates whether primitives should be appended using their language
/// alias, as opposed to their fully qualified framework name.
/// </param>
/// <param name="UseGloballyQualifiedName">
/// Indicates whether type names should be globally qualified.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly record struct TypeNameOptions(
        Boolean UseTypeAliases = true,
        Boolean UseGloballyQualifiedName = true)
{
    /// <summary>
    /// Gets the default instance.
    /// </summary>
    public static TypeNameOptions Default { get; } = new(UseTypeAliases: true, UseGloballyQualifiedName: true);
}
