// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Implements an arbitrary component.
/// </summary>
/// <remarks>
/// This type does not support value equality.
/// </remarks>
/// <param name="append">
/// The callback to invoke when appending to a builder.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
readonly record struct StrategyComponent(Action<CSharpSourceBuilder, CancellationToken> append) : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        append.Invoke(builder, cancellationToken);
    }

    /// <inheritdoc />
    public Boolean Equals(StrategyComponent other) =>
            throw new NotSupportedException("Equals is not supported on this type.");

    /// <inheritdoc />
    public override Int32 GetHashCode() =>
            throw new NotSupportedException("GetHashCode is not supported on this type.");
}
