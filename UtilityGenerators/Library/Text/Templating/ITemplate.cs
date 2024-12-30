namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;

/// <summary>
/// Represents a component that may be rendered to a buffer.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal interface ITemplate
{
    /// <summary>
    /// Renders the template to a buffer, using the indentation provided.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to render the templates characters to.
    /// </param>
    /// <param name="indentation">
    /// The indentation to apply to text appended.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request rendering to be cancelled.
    /// </param>
    void Render(ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken);
}
