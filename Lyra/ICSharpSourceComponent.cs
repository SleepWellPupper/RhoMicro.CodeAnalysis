// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System.Threading;

/// <summary>
/// Represents a component that may be appended to a <see cref="CSharpSourceBuilder"/>.
/// </summary>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal interface ICSharpSourceComponent
{
    /// <summary>
    /// Appends the component to a builder. 
    /// </summary>
    /// <param name="builder">
    /// The builder to append the component to.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request appending to be cancelled.
    /// </param>
    void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default);
}
