namespace RhoMicro.CodeAnalysis;
using System;

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
    public TemplateAttribute([MapToProperty(nameof(TemplateString))]String templateString) => TemplateString = templateString;
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
}