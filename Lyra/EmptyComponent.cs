// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Implements an empty component.
/// </summary>
/// <remarks>
/// This type supports value equality.
/// </remarks>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
sealed class EmptyComponent : ICSharpSourceComponent
{
    private EmptyComponent(){}

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static EmptyComponent Instance { get; } = new();

    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
    }
}
