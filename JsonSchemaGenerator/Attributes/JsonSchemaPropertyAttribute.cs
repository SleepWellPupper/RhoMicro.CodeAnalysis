// SPDX-License-Identifier: MPL-2.0

#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Provides additional information about the targeted property property to the generator.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[IncludeFile]
#endif
internal sealed partial class JsonSchemaPropertyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the title of the property.
    /// </summary>
    public String Title { get; set; } = String.Empty;
    /// <summary>
    /// Gets or sets the description of the property.
    /// </summary>
    public String Description { get; set; } = String.Empty;
}
