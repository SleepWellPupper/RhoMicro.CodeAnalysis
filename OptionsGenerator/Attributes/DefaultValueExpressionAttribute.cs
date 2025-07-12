// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

/// <summary>
/// Provides an expression for initializing implementations of the target property.
/// </summary>
#if RHOMICRO_CODEANALYSIS_OPTIONSGENERATOR
[IncludeFile]
[GenerateFactory(GenerateModelTypeAsStruct = true)]
#endif
#if GENERATOR
[NonEquatable]
#endif
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed partial class DefaultValueExpressionAttribute : Attribute
{
    /// <summary>
    /// Provides an expression for initializing implementations of the target property.
    /// </summary>
    /// <param name="expression">
    /// The expression to initialize the property with.
    /// </param>
    public DefaultValueExpressionAttribute(
#if RHOMICRO_CODEANALYSIS_OPTIONSGENERATOR
        [MapToProperty(nameof(Expression))]
#endif
    String expression) => Expression = expression;
    /// <summary>
    /// Gets the expression to initialize the property with. If left empty, no
    /// initializer will be used.
    /// </summary>
#if RHOMICRO_CODEANALYSIS_OPTIONSGENERATOR
    [RhoMicro.CodeAnalysis.DefaultValue("default!")]
#endif
    public String Expression { get; }
}