// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

/// <summary>
/// Represents an empty template. Rendering instance will always yield an empty string.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed class EmptyTemplate : ITemplate
{
    private EmptyTemplate() { }
    public static EmptyTemplate Instance { get; } = new();
    public void Render<TBody>(ref TemplateRenderer context, TBody body, CancellationToken cancellationToken)
        where TBody : ITemplate 
        => cancellationToken.ThrowIfCancellationRequested();
}