namespace RhoMicro.CodeAnalysis;
using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

/// <summary>
/// Generates members disallowing equality operations on types.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class NonEquatableGenerator : IIncrementalGenerator
{
    private const String _attributeMetadataName = "RhoMicro.CodeAnalysis.NonEquatableAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            _attributeMetadataName,
            static (_, _) => true,
            static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if(ctx.TargetSymbol is not INamedTypeSymbol target)
                    return null;

                using var modelCtx = ModelCreationContext.CreateDefault(ct);
                var model = NamedTypeModel.Create(target, in modelCtx);

                return model;
            })
            .Where(static m => m is not null)
            .Select(static (m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
                {
                    AmbientCancellationToken = ct,
                    GeneratorName = typeof(NonEquatableGenerator).FullName
                });

                m!.BuildStrings(sourceBuilder, out var hintName, out var displayName, ct);

                sourceBuilder
                    .AppendLine("/// <inheritdoc/>")
                    .AppendCore("public ");

                if(m.Kind == PartialTypeKindModel.Class || m.Kind == PartialTypeKindModel.Record)
                    sourceBuilder.AppendCore("sealed ");

                sourceBuilder.Append("override bool Equals(object obj) => throw new global::System.NotSupportedException(\"")
                    .Append(displayName)
                    .AppendLine(".Equals(object) is not supported.\");")
                    .AppendLine("/// <inheritdoc/>")
                    .Append("public bool Equals(").Append(displayName).Append(" obj) => throw new global::System.NotSupportedException(\"")
                    .Append(displayName)
                    .Append(".Equals(").Append(displayName).AppendLine(") is not supported.\");")
                    .AppendLine("/// <inheritdoc/>")
                    .AppendCore("public ");

                if(m.Kind == PartialTypeKindModel.Class || m.Kind == PartialTypeKindModel.Record)
                    sourceBuilder.AppendCore("sealed ");

                sourceBuilder.Append("override int GetHashCode() => throw new global::System.NotSupportedException(\"")
                    .Append(displayName)
                    .Append(".GetHashCode() is not supported.\");")
                    .AppendLineCore();

                var source = sourceBuilder.CloseAllBlocks().ToString();

                return (hintName, source);
            });

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }
}
