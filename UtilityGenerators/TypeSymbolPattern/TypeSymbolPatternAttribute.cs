// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;
using System;

using Microsoft.CodeAnalysis;

/// <summary>
/// Marks the targeted partial method for type symbol pattern generation. The
/// pattern generated will match an instance of <see cref="ITypeSymbol"/>
/// against the defined type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS && !RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS || RHOMICRO_CODEANALYSIS_UTILITYGENERATORS_DEV
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
internal sealed partial class TypeSymbolPatternAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type">
    /// The type the generated implementation should match an instance of <see
    /// cref="ITypeSymbol"/> against.
    /// </param>
    public TypeSymbolPatternAttribute([MapToProperty(nameof(Type))] Type type) => Type = type;
    /// <summary>
    /// Gets the type the generated implementation should match an instance of
    /// <see cref="ITypeSymbol"/> against.
    /// </summary>
    public Type Type { get; }
    /// <summary>
    /// Gets or sets a value indicating whether the generated pattern should
    /// consider type arguments. The default value is <see langword="true"/>.
    /// </summary>
    [DefaultValue(true)]
    public Boolean CheckTypeArguments { get; set; } = true;
}
