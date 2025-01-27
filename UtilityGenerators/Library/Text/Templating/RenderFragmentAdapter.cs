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
    public void Render<TBody>(ref TemplateRenderer renderer, TBody body)
        where TBody : ITemplate
        => fragment.Invoke(ref renderer);
}
