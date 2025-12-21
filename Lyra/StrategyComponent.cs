// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Implements an arbitrary component.
/// </summary>
/// <remarks>
/// This type does not support value equality.
/// </remarks>
/// <param name="Append">
/// The callback to invoke when appending to a builder.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
readonly record struct StrategyComponent(Action<CSharpSourceBuilder, CancellationToken> Append) : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Append.Invoke(builder, cancellationToken);
    }

    /// <inheritdoc />
    public Boolean Equals(StrategyComponent other) =>
        throw new NotSupportedException("Equals is not supported on this type.");

    /// <inheritdoc />
    public override Int32 GetHashCode() =>
        throw new NotSupportedException("GetHashCode is not supported on this type.");
}

/// <summary>
/// Implements an arbitrary component that uses state.
/// </summary>
/// <remarks>
/// This type does not support value equality.
/// </remarks>
/// <param name="State">
/// The state used by the component.
/// </param>
/// <param name="Append">
/// The callback to invoke when appending to a builder.
/// </param>
/// <typeparam name="TState">
/// The type of state used by the component.
/// </typeparam>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
readonly record struct StrategyComponent<TState>(
    TState State,
    Action<TState, CSharpSourceBuilder, CancellationToken> Append) : ICSharpSourceComponent
{
    /// <inheritdoc />
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Append.Invoke(State, builder, cancellationToken);
    }

    /// <inheritdoc />
    public Boolean Equals(StrategyComponent<TState> other) =>
        throw new NotSupportedException("Equals is not supported on this type.");

    /// <inheritdoc />
    public override Int32 GetHashCode() =>
        throw new NotSupportedException("GetHashCode is not supported on this type.");
}
