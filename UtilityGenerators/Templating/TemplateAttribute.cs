// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using System;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

/// <summary>
/// Marks the target type for template generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS && !RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS || RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
internal sealed partial class TemplateAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="templateString">
    /// The template string to generate a template class implementation from.
    /// </param>
    public TemplateAttribute([MapToProperty(nameof(TemplateString))] String templateString) => TemplateString = templateString;
    /// <summary>
    /// Gets the template string used for the targeted template class.
    /// </summary>
    public String TemplateString { get; }
    /// <summary>
    /// Gets or sets a value indicating whether to generate an implementation of
    /// <see cref="Object.ToString"/> that returns a render of the template. The
    /// default value is <see langword="true"/>.
    /// </summary>
    [DefaultValue(true)]
    public Boolean GenerateToString { get; set; } = true;
    /// <summary>
    /// Gets or sets the name of the renderer parameter of the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__renderer</c>.
    /// </summary>
    [DefaultValue("__renderer")]
    public String RendererParameterName { get; set; } = "__renderer";
    /// <summary>
    /// Gets or sets the name of the template string constant of the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__template</c>.
    /// </summary>
    [DefaultValue("__template")]
    public String TemplateConstName { get; set; } = "__template";
    /// <summary>
    /// Gets or sets the name of the body parameter of the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__body</c>.
    /// </summary>
    [DefaultValue("__body")]
    public String BodyParameterName { get; set; } = "__body";
    /// <summary>
    /// Gets or sets the name of the body type parameter of the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__TBody</c>.
    /// </summary>
    [DefaultValue("__TBody")]
    public String BodyParameterTypeName { get; set; } = "__TBody";
    /// <summary>
    /// Gets or sets the newline to use when rendering line breaks. The
    /// default value is <see cref="Newline.Lf"/>.
    /// </summary>
    [DefaultValue((Int32)Newline.Lf)]
    public Newline Newline { get; set; } = Newline.Lf;
    /// <summary>
    /// Gets or sets the name of synthesized render fragments in the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__fragment</c>.
    /// </summary>
    [DefaultValue("__fragment")]
    public String FragmentName { get; set; } = "__fragment";
    /// <summary>
    /// Gets or sets the name of the cancellation token parameter of the synthesized <see
    /// cref="global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate.Render{TBody}(ref
    /// Library.Text.Templating.TemplateRenderer, TBody, CancellationToken)"/> method. The
    /// default value is <c>__cancellationToken</c>.
    /// </summary>
    [DefaultValue("__cancellationToken")]
    public String CancellationTokenParameterName { get; set; } = "__cancellationToken";
    /// <summary>
    /// Gets or sets the using statements to include in the generated template file.
    /// The default values are:
    /// <list type="bullet">
    /// <item><c>static global::RhoMicro.CodeAnalysis.Library.Text.Templating.Indentations</c></item>
    /// </list>
    /// </summary>
    public String[]? Usings { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to generate a structural
    /// representation and debugging information of the template for debugging
    /// purposes. The default value is <see langword="false"/>.
    /// </summary>
    [DefaultValue(false)]
    public Boolean GenerateDebugInfo { get; set; } = false;
}
