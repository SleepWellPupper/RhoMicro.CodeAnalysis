#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Provides access to generated json schemata for use by other tools.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
#if GENERATOR
[GenerateFactory]
[IncludeFile]
#endif
internal sealed partial class GeneratedJsonSchemaAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="schema">
    /// The generated schema.
    /// </param>
    public GeneratedJsonSchemaAttribute(String schema) => Schema = schema;
    /// <summary>
    /// Gets the generated schema.
    /// </summary>
    public String Schema { get; }
}