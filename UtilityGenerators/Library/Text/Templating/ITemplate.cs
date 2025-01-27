namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
/// <summary>
/// Represents a component that may be rendered to a buffer.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal interface ITemplate
{
    /// <summary>
    /// Renders the template.
    /// </summary>
    /// <param name="renderer">
    /// The renderer to render the template with.
    /// </param>
    /// <param name="body">
    /// The body to render inside of the template. If the template
    /// implementation does not support bodies, the parameter is ignored.
    /// </param>
    void Render<TBody>(ref TemplateRenderer renderer, TBody body)
        where TBody : ITemplate;
}
