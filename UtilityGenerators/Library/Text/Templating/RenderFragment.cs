namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

/// <summary>
/// Renders a template.
/// </summary>
/// <param name="renderer">
/// The renderer to render the template with.
/// </param>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal delegate void RenderFragment(ref TemplateRenderer renderer);
