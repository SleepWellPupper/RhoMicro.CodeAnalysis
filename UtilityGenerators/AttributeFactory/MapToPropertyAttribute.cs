namespace RhoMicro.CodeAnalysis;

using System;

/// <summary>
/// Maps the target constructor parameter onto a property.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[NonEquatable]
[IncludeFile]
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
internal sealed partial class MapToPropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property to map the targeted parameter onto.
    /// </param>
    public MapToPropertyAttribute([MapToProperty(nameof(PropertyName))] String propertyName) => PropertyName = propertyName;
    /// <summary>
    /// Gets the name of the property to map the targeted parameter onto.
    /// </summary>
    public String PropertyName { get; }
}
