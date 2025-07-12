// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

/// <summary>
/// Adapts a render fragment onto <see cref="ITemplate"/>.
/// </summary>
/// <param name="fragment">
/// The fragment to adapt.
/// </param>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal readonly struct RenderFragmentAdapter(RenderFragment fragment) : ITemplate
{
    public void Render<TBody>(ref TemplateRenderer renderer, TBody body, CancellationToken cancellationToken)
        where TBody : ITemplate
    {
        cancellationToken.ThrowIfCancellationRequested();

        fragment.Invoke(ref renderer, cancellationToken);
    }
}
