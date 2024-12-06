#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Marks the targeted type for json schema generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[IncludeFile]
#endif
internal sealed partial class JsonSchemaAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the json schema id of the generated schema. If left empty
    /// or set to <see langword="null"/>, a schema id will be generated based on
    /// the annotated type.
    /// </summary>
    public String? Id { get; set; }
    /// <summary>
    /// Gets or sets the title of the schema.
    /// </summary>
    public String Title { get; set; } = String.Empty;
    /// <summary>
    /// Gets or sets the description of the schema.
    /// </summary>
    public String Description { get; set; } = String.Empty;
}
