// SPDX-License-Identifier: MPL-2.0

#nullable enable
#pragma warning disable
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace RhoMicro.CodeAnalysis
{
    internal partial class JsonSchemaPropertyAttribute
    {
        /// <summary>
        /// Attempts to create an instance of <see cref = "JsonSchemaPropertyAttribute"/> based on an
        /// instance of <see cref = "AttributeData"/>.
        /// </summary>
        /// <param name = "data">The attribute data to try and create an instance from.</param>
        /// <param name = "result">
        /// Will contain the created instance, if one could be created; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an instance could be created; otherwise, <see langword="false"/>.
        /// </returns>
        public static Boolean TryCreate(AttributeData data, out JsonSchemaPropertyAttribute? result)
        {
            result = null;
            if(data.AttributeClass == null
                || data.AttributeClass.MetadataName != "JsonSchemaPropertyAttribute"
                || data.AttributeClass.ContainingNamespace.ToDisplayString() != "RhoMicro.CodeAnalysis")
            {
                return false;
            }

            var ctorArgs = data.ConstructorArguments;
            switch(ctorArgs.Length)
            {
                case 0:
                    result = new JsonSchemaPropertyAttribute();
                    break;
                default:
                    return false;
            }

            var propArgs = data.NamedArguments;
            foreach(var propArg in propArgs)
            {
                switch(propArg.Key)
                {
                    case "Title":
                        try
                        {
                            result.Title = (string)propArg.Value.Value;
                        } catch
                        {
                        }

                        break;
                    case "Description":
                        try
                        {
                            result.Description = (string)propArg.Value.Value;
                        } catch
                        {
                        }

                        break;
                    default:
                        return false;
                }
            }

            return true;
            static Object[] getValues(TypedConstant constant) => constant.Value != null ? new Object[]
            {
                constant.Value
            }

            : constant.IsNull ? null : constant.Values.Select(getValues).ToArray();
        }

        public const String MetadataName = "RhoMicro.CodeAnalysis.JsonSchemaPropertyAttribute";
        public const String SourceText = """
#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Provides additional information about the targeted property property to the generator.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[GenerateFactory]
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

""";
    }

    /// <summary>
    /// Contains extension methods pertaining to the <see cref = "JsonSchemaPropertyAttribute"/> type.
    /// </summary>
    internal static class JsonSchemaPropertyAttributeExtensions
    {
        /// <summary>
        /// Filters and projects an enumeration of <see cref = "AttributeData"/> onto
        /// instances of <see cref = "JsonSchemaPropertyAttribute"/>.
        /// </summary>
        /// <param name = "data">The attribute data to filter.</param>
        /// <returns>The filtered and projected data.</returns>
        public static IEnumerable<JsonSchemaPropertyAttribute> OfJsonSchemaPropertyAttribute(this IEnumerable<AttributeData> data) => data.Select(d => (Success: JsonSchemaPropertyAttribute.TryCreate(d, out var a), Attribute: a)).Where(t => t.Success).Select(t => t.Attribute);
        /// <summary>
        /// Attempts to retrieve the first instance of <see cref = "JsonSchemaPropertyAttribute"/> from a symbol.
        /// </summary>
        /// <param name = "symbol">The symbol whose attributes to scan for an instance of <see cref = "JsonSchemaPropertyAttribute"/>.</param>
        /// <param name = "attribute">The attribute retrieved, if one could be located; otherwise, <see langword="null"/>.</param>
        public static Boolean TryGetFirstJsonSchemaPropertyAttribute(this ISymbol symbol, out JsonSchemaPropertyAttribute attribute)
        {
            attribute = symbol.GetAttributes().OfJsonSchemaPropertyAttribute().FirstOrDefault();
            return attribute != null;
        }

        public static IncrementalValuesProvider<T> ForJsonSchemaPropertyAttribute<T>(this SyntaxValueProvider provider, Func<SyntaxNode, CancellationToken, bool> predicate, Func<GeneratorAttributeSyntaxContext, CancellationToken, T> transform) => provider.ForAttributeWithMetadataName("RhoMicro.CodeAnalysis.JsonSchemaPropertyAttribute", predicate, transform);
    }
}
