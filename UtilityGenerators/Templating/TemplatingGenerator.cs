namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;

/// <summary>
/// Generates template classes from string templates.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class TemplatingGenerator : IIncrementalGenerator
{
    private const String _attributeMetadataName = "RhoMicro.CodeAnalysis.TemplateAttribute";
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName<(NamedTypeModel, TemplateString, TemplateAttribute.Model)?>(
            _attributeMetadataName,
            static (n, _) => n is RecordDeclarationSyntax or ClassDeclarationSyntax or StructDeclarationSyntax,
            static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if(ctx is not
                    {
                        TargetSymbol: INamedTypeSymbol target,
                        Attributes: [{ } attributeData, ..]
                    }
                    || !attributeData.TryGetTemplateAttributeModel(out var attribute, cancellationToken: ct)
                    || attributeData.ApplicationSyntaxReference?.GetSyntax(ct) is not AttributeSyntax
                    {
                        ArgumentList.Arguments: [{ Expression: LiteralExpressionSyntax { Token: var token } }, ..]
                    })
                {
                    return null;
                }

                using var modelCtx = ModelCreationContext.CreateDefault(ct);
                var templateString = TemplateString.Create(token, ct);
                var typeModel = NamedTypeModel.Create(target, in modelCtx);
                var result = (typeModel, templateString, attribute);

                return result;
            })
            .Where(static t => t.HasValue)
            .Select(static (t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var (type, templateString, attribute) = t!.Value;
                using var ctx = ModelCreationContext.CreateDefault(ct);
                var scanResult = Lexer.Scan(templateString, in ctx);

                return (type, scanResult, attribute);
            })
            .Select(static (t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var (type, scanResult, attribute) = t;
                using var ctx = ModelCreationContext.CreateDefault(ct);
                var parseResult = Parser.Parse(scanResult, in ctx);

                return (type, parseResult, attribute);
            });

        var isbProvider = provider.Select(IsbImpl);
        context.RegisterSourceOutput(isbProvider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }
    private static (String hintName, String source) IsbImpl(
        (NamedTypeModel type, ParseResult parseResult, TemplateAttribute.Model attribute) t,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
        {
            AmbientCancellationToken = ct,
            GeneratorName = typeof(TemplatingGenerator).FullName
        });

        var (type, parseResult, attribute) = t;
        var scanResult = parseResult.ScanResult;
        var template = parseResult.Syntax;
        var templateString = scanResult.TemplateString;

        foreach(var u in attribute.Usings)
            sourceBuilder.Append("using ").Append(u).Append(';').AppendLineCore();

        type.BuildStrings(
            sourceBuilder,
            out var hintName,
            out _,
            ["global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate"],
            ct);

        if(attribute.GenerateToString)
        {
            sourceBuilder
                .AppendLine("public override string ToString() => global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render(this);")
                .Append(
                    "public string ToString(global::System.Threading.CancellationToken cancellationToken) => " +
                    "global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render(this, cancellationToken);"
                    ).AppendLineCore();
        }

        sourceBuilder
            .Append("public void Render<")
            .Append(attribute.BodyParameterTypeName)
            .Append(">(ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer ")
            .Append(attribute.RendererParameterName).Append(", ")
            .Append(attribute.BodyParameterTypeName).Append(' ').Append(attribute.BodyParameterName).AppendLine(" )")
            .Append("where ").Append(attribute.BodyParameterTypeName).Append(" : global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate")
            .OpenBracesBlock()
            .Append(attribute.RendererParameterName).AppendLine(".ThrowIfCancellationRequested();")
            .AppendLine()
            .Append("// This template was taken from: ")
            .Append(templateString.Path)
            .Append('(').Append(templateString.Start.Line.ToString()).Append(',').Append(templateString.Start.Character.ToString()).AppendLine(')')
            .Append("const string __template =").AppendLineCore();

        var detentCount = 0;
        for(; detentCount < sourceBuilder.OpenBlocks; detentCount++)
            sourceBuilder.DetentCore();

        var quotes = new String('"', scanResult.RequiredQuotes);
        sourceBuilder
            .AppendLine(quotes)
            .AppendLine(attribute.TemplateString)
            .Append(quotes)
            .AppendCore(";");

        for(; detentCount > 0; detentCount--)
            sourceBuilder.IndentCore();

        sourceBuilder.AppendLine().AppendLineCore();

        if(attribute.GenerateStructuralRepresentation)
        {
            sourceBuilder.Append("/* Structural Representation:").AppendLineCore();

            detentCount = 0;
            for(; detentCount < sourceBuilder.OpenBlocks; detentCount++)
                sourceBuilder.DetentCore();

            var structuralRepresentation = template.ToAstString(ct);
            sourceBuilder.Append(structuralRepresentation).AppendLineCore();

            for(; detentCount > 0; detentCount--)
                sourceBuilder.IndentCore();

            sourceBuilder.Append("*/").AppendLineCore();
        }

        var source = sourceBuilder.CloseAllBlocks().ToString();

        return (hintName, source);
    }
}