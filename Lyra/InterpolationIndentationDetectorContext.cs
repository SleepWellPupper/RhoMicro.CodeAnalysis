// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

/// <summary>
/// Provides contextual apis to instances of <see cref="IInterpolationIndentationDetector"/>.
/// </summary>
/// <param name="options">
/// The builder options used by the invoking builder.
/// </param>
/// <param name="buffers">
/// An object from which buffers may be rented.
/// The lifetime of rented buffers is tied to the lifetime of the invoking builder,
/// meaning they will be returned once the builder has been disposed.
/// </param>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal readonly struct InterpolationIndentationDetectorContext(CSharpSourceBuilderOptions options, RentedArrayLifetime buffers)
{
    /// <summary>
    /// Gets the options used by the invoking builder.
    /// </summary>
    public CSharpSourceBuilderOptions Options { get; } = options;
    /// <summary>
    /// Gets an object from which buffers may be rented.
    /// The lifetime of rented buffers is tied to the lifetime of the invoking builder,
    /// meaning they will be returned once the builder has been disposed.
    /// </summary>
    public RentedArrayLifetime Buffers { get; } = buffers;
}
