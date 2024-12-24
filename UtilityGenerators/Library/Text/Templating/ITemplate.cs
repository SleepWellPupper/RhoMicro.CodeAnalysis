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
    /// Renders the template to a buffer.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to render the templates characters to.
    /// </param>
    void Render(ref DynamicallyAllocatedBuffer<Char> buffer);
}
