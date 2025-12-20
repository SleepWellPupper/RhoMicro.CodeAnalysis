// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Library.Text.Templating;
using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Generates template classes from string templates.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class TemplatingGenerator : IIncrementalGenerator
{
    private const String _attributeMetadataName = "RhoMicro.CodeAnalysis.TemplateAttribute";
    private const Int32 _parserTimeout = 100;

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName<(NamedTypeModel, TemplateString, TemplateAttribute.Model)?>(
                _attributeMetadataName,
                static (n, _) => n is RecordDeclarationSyntax or ClassDeclarationSyntax or StructDeclarationSyntax,
                static (ctx, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    if (ctx is not
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
                    var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    cts.CancelAfter(_parserTimeout);
                    var templateString = TemplateString.Create(token, cts.Token);
                    var typeModel = NamedTypeModel.Create(target, in modelCtx);
                    var result = (typeModel, templateString, attribute);

                    return result;
                })
            .Where(static t => t.HasValue)
            .Select(static (t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var (type, templateString, attribute) = t!.Value;
                var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(_parserTimeout);
                using var ctx = ModelCreationContext.CreateDefault(cts.Token);
                var scanResult = Lexer.Scan(templateString, attribute.NewlineValue.Length, in ctx);

                return (type, scanResult, attribute);
            })
            .Select(static (t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var (type, scanResult, attribute) = t;
                var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(_parserTimeout);
                using var ctx = ModelCreationContext.CreateDefault(cts.Token);
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
            AmbientCancellationToken = ct, GeneratorName = typeof(TemplatingGenerator).FullName
        });

        var (type, parseResult, attribute) = t;
        var scanResult = parseResult.ScanResult;
        var template = parseResult.Syntax;
        var templateString = scanResult.TemplateString;
        var diagnostics = parseResult.AllDiagnostics;

        foreach (var u in attribute.Usings ?? [])
        {
            sourceBuilder.Append("using ").Append(u).Append(';').AppendLineCore();
        }

        type.BuildStrings(
            sourceBuilder,
            out var hintName,
            out var displayString,
            ["global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate"],
            ct);

        if (attribute.GenerateToString)
        {
            sourceBuilder
                .AppendLine("public override string ToString()")
                .Indent()
                .AppendLine("=> global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render(this);")
                .AppendLine()
                .Detent()
                .Append("public string RenderToString<")
                .Append(attribute.BodyParameterTypeName)
                .Append(">(in ")
                .Append(attribute.BodyParameterTypeName).AppendLine(" body)")
                .Indent()
                .Append("where ").Append(attribute.BodyParameterTypeName)
                .AppendLine(" : global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate")
                .Append("=> global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render<")
                .Append(displayString)
                .Append(", ")
                .Append(attribute.BodyParameterTypeName)
                .AppendLine(">(this, body);")
                .AppendLine()
                .Detent()
                .Append("public string RenderToString<")
                .Append(attribute.BodyParameterTypeName)
                .Append(">(in ")
                .Append(attribute.BodyParameterTypeName)
                .AppendLine(" body, global::System.Threading.CancellationToken cancellationToken)")
                .Indent()
                .Append("where ").Append(attribute.BodyParameterTypeName)
                .AppendLine(" : global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate")
                .Append("=> global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render<")
                .Append(displayString)
                .Append(", ")
                .Append(attribute.BodyParameterTypeName)
                .AppendLine(">(this, body, cancellationToken);")
                .AppendLine()
                .Detent()
                .AppendLine(
                    "public string RenderToString(" +
                    "global::System.Threading.CancellationToken cancellationToken)")
                .Indent()
                .AppendLine("=> " +
                            "global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer.Render(" +
                            "this, cancellationToken" +
                            ");")
                .AppendLine()
                .DetentCore();
        }

        sourceBuilder
            .Append("public void Render<")
            .Append(attribute.BodyParameterTypeName)
            .AppendLine(">(")
            .Indent()
            .Append("ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRenderer ")
            .Append(attribute.RendererParameterName).AppendLine(',')
            .Append(attribute.BodyParameterTypeName).Append(' ')
            .Append(attribute.BodyParameterName).AppendLine(',')
            .Append("global::System.Threading.CancellationToken ")
            .Append(attribute.CancellationTokenParameterName).AppendLine(')')
            .Append("where ").Append(attribute.BodyParameterTypeName)
            .Append(" : global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate")
            .Detent()
            .OpenBracesBlock()
            .Append(attribute.CancellationTokenParameterName).AppendLine(".ThrowIfCancellationRequested();")
            .AppendLine()
            .Append("// This template was taken from: ")
            .Append(templateString.Path)
            .Append('(').Append(templateString.Start.Line.ToString()).Append(',')
            .Append(templateString.Start.Character.ToString()).AppendLine(')')
            .Append("const string __template =").AppendLineCore();

        var detentCount = 0;
        for (; detentCount < sourceBuilder.OpenBlocks; detentCount++)
        {
            sourceBuilder.DetentCore();
        }

        var quotes = new String('"', scanResult.RequiredQuotes);
        sourceBuilder
            .Append(quotes)
            .AppendLineCore();

        template.Accept(
            new TemplateStringReconstructionVisitor(
                newline: attribute.NewlineValue,
                sourceBuilder,
                ct));

        sourceBuilder
            .AppendLine()
            .Append(quotes)
            .AppendCore(";");

        for (; detentCount > 0; detentCount--)
        {
            sourceBuilder.IndentCore();
        }

        sourceBuilder.AppendLine().AppendLineCore();

        AppendDebugComment(sourceBuilder, attribute, template, ct);
        AppendDiagnostics(sourceBuilder, diagnostics, ct);
        AppendTemplate(sourceBuilder, template, attribute, templateString, ct);

        var source = sourceBuilder.CloseAllBlocks().ToString();

        return (hintName, source);
    }

    private static void AppendTemplate(
        IndentedStringBuilder sourceBuilder,
        TemplateSyntax template,
        TemplateAttribute.Model attribute,
        TemplateString templateString,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        template.Accept(new SourceGeneratingTemplateVisitor(sourceBuilder, attribute, templateString, ct));
    }

    private static void AppendDiagnostics(IndentedStringBuilder sourceBuilder,
                                          IEnumerable<Templating.Diagnostic> diagnostics, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        foreach (var diagnostic in diagnostics)
        {
            sourceBuilder
                .Append("// ")
                .Append(diagnostic.ToString())
                .AppendLineCore();
        }
    }

    private static void AppendDebugComment(
        IndentedStringBuilder sourceBuilder,
        TemplateAttribute.Model attribute,
        TemplateSyntax template,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!attribute.GenerateDebugInfo)
        {
            return;
        }

        var xmlTree = template.ToCommentXmlTreeString(ct);

        sourceBuilder.Append(xmlTree).AppendLineCore();
    }
}
