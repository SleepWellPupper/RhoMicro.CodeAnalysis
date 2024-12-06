#pragma warning disable
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace RhoMicro.CodeAnalysis
{
    internal partial class JsonSchemaExcludeAttribute
    {
        /// <summary>
        /// Attempts to create an instance of <see cref = "JsonSchemaExcludeAttribute"/> based on an 
        /// instance of <see cref = "AttributeData"/>.
        /// </summary>
        /// <param name = "data">The attribute data to try and create an instance from.</param>
        /// <param name = "result">
        /// Will contain the created instance, if one could be created; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an instance could be created; otherwise, <see langword="false"/>.
        /// </returns>
        public static Boolean TryCreate(AttributeData data, out JsonSchemaExcludeAttribute? result)
        {
            result = null;
            if(data.AttributeClass == null 
                || data.AttributeClass.MetadataName != "JsonSchemaExcludeAttribute" 
                || data.AttributeClass.ContainingNamespace.ToDisplayString() != "RhoMicro.CodeAnalysis")
            {
                return false;
            }

            var ctorArgs = data.ConstructorArguments;
            switch(ctorArgs.Length)
            {
                case 0:
                    result = new JsonSchemaExcludeAttribute();
                    break;
                default:
                    return false;
            }

            var propArgs = data.NamedArguments;
            foreach(var propArg in propArgs)
            {
                switch(propArg.Key)
                {
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

        public const String MetadataName = "RhoMicro.CodeAnalysis.JsonSchemaExcludeAttribute";
        public const String SourceText = """
#pragma warning disable
namespace RhoMicro.CodeAnalysis;
using System;

/// <summary>
/// Marks the annotated property to be excluded from json schema generation.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if GENERATOR
[GenerateFactory]
[IncludeFile]
#endif
internal sealed partial class JsonSchemaExcludeAttribute : Attribute { }
""";
    }

    /// <summary>
    /// Contains extension methods pertaining to the <see cref = "JsonSchemaExcludeAttribute"/> type.
    /// </summary>
    internal static class JsonSchemaExcludeAttributeExtensions
    {
        /// <summary>
        /// Filters and projects an enumeration of <see cref = "AttributeData"/> onto 
        /// instances of <see cref = "JsonSchemaExcludeAttribute"/>.
        /// </summary>
        /// <param name = "data">The attribute data to filter.</param>
        /// <returns>The filtered and projected data.</returns>
        public static IEnumerable<JsonSchemaExcludeAttribute> OfJsonSchemaExcludeAttribute(this IEnumerable<AttributeData> data) => data.Select(d => (Success: JsonSchemaExcludeAttribute.TryCreate(d, out var a), Attribute: a)).Where(t => t.Success).Select(t => t.Attribute);
        /// <summary>
        /// Attempts to retrieve the first instance of <see cref = "JsonSchemaExcludeAttribute"/> from a symbol.
        /// </summary>
        /// <param name = "symbol">The symbol whose attributes to scan for an instance of <see cref = "JsonSchemaExcludeAttribute"/>.</param>
        /// <param name = "attribute">The attribute retrieved, if one could be located; otherwise, <see langword="null"/>.</param>
        public static Boolean TryGetFirstJsonSchemaExcludeAttribute(this ISymbol symbol, out JsonSchemaExcludeAttribute attribute)
        {
            attribute = symbol.GetAttributes().OfJsonSchemaExcludeAttribute().FirstOrDefault();
            return attribute != null;
        }

        public static IncrementalValuesProvider<T> ForJsonSchemaExcludeAttribute<T>(this SyntaxValueProvider provider, Func<SyntaxNode, CancellationToken, bool> predicate, Func<GeneratorAttributeSyntaxContext, CancellationToken, T> transform) => provider.ForAttributeWithMetadataName("RhoMicro.CodeAnalysis.JsonSchemaExcludeAttribute", predicate, transform);
    }
}