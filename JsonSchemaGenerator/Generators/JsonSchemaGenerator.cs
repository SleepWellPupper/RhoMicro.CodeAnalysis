// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Generators;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Generated;
using RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;
using RhoMicro.CodeAnalysis.Library.Text;

/// <summary>
/// Generates json schemata from class definitions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class JsonSchemaGenerator : IIncrementalGenerator
{
    private const String _hintName = "RhoMicro_CodeAnalysis_JsonSchemaGenerator_JsonSchemaGenerator_Schemata.g.cs";
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var schemataProvider = context.SyntaxProvider.ForJsonSchemaAttribute(
            static (_, _) => true,
            static (ctx, ct) =>
            {
                var attributes = ctx.Attributes;
                var symbol = ctx.TargetSymbol;

                var schemaBuilder = new JsonSchemaModel();
                schemaBuilder.SubSchema().Populate(ctx, ct);
                var result = schemaBuilder.Build(includeId: true, ct);

                return result;
            })
            .Where(m => m is not null)
            .Collect()
            .Select((s, ct) =>
            {
                var resultBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
                {
                    AmbientCancellationToken = ct,
                    GeneratorName = typeof(JsonSchemaGenerator).FullName,
                    PrependMarkerComment = false
                });

                for(var i = 0; i < s.Length; i++)
                {
                    resultBuilder.Append("[assembly: global::")
                        .Append(typeof(GeneratedJsonSchemaAttribute).FullName)
                        .Append("(\"\"\"")
                        .AppendModel(s[i]!)
                        .Append("\"\"\")]")
                        .AppendLineCore();
                }

                var source = resultBuilder.ToString();

                var result = (hintName: _hintName, source);

                return result;
            });

        context.RegisterSourceOutput(schemataProvider, (ctx, schema) => ctx.AddSource(schema.hintName, schema.source));
        IncludedFileSources.RegisterToContext(context);
    }
}
