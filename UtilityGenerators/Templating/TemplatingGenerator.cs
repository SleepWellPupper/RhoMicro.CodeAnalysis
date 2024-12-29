namespace RhoMicro.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;
using RhoMicro.CodeAnalysis.Templating;

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
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName<(NamedTypeModel type, TemplateAttribute.Model attribute)?>(
            _attributeMetadataName,
            static (_, _) => true,
            static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if(ctx is not
                    {
                        TargetSymbol: INamedTypeSymbol target,
                        Attributes: [{ } attributeData, ..]
                    } || !attributeData.TryGetTemplateAttributeModel(out var attribute, cancellationToken: ct))
                {
                    return null;
                }

                using var modelCtx = ModelCreationContext.CreateDefault(ct);
                var typeModel = NamedTypeModel.Create(target, in modelCtx);
                var result = (typeModel, attribute);

                return result;
            }).Where(t => t.HasValue)
            .Select((t, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                using var ctx = ModelCreationContext.CreateDefault(ct);
                var template = TemplateSyntaxModel.Parse(t!.Value.attribute.TemplateString, in ctx);

                return (t.Value.type, template, t.Value.attribute);
            })
            .Select(IsbImpl);

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }
    
    private static (String hintName, String source) IsbImpl((NamedTypeModel type, TemplateSyntaxModel template, TemplateAttribute.Model attribute) t, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
        {
            AmbientCancellationToken = ct,
            GeneratorName = typeof(TemplatingGenerator).FullName
        });

        var (type, template, attribute) = t;

        type.BuildStrings(
            sourceBuilder,
            out var hintName,
            out _,
            ["global::RhoMicro.CodeAnalysis.Library.Text.Templating.ITemplate"],
            ct);

        if(attribute.GenerateToString)
        {
            sourceBuilder
                .Append("public override unsafe string ToString()")
                .OpenBracesBlock()
                .AppendLine("using var buffer = new global::RhoMicro.CodeAnalysis.Library.Text.Templating.DynamicallyAllocatedBuffer<char>();")
                .AppendLine("this.Render(ref buffer);")
                .AppendLine("fixed(char* chars = buffer.Span)")
                .Indent().AppendLine("return new(chars, 0, buffer.Span.Length);").Detent()
                .CloseBlockCore();
        }

        sourceBuilder
            .Append("public void Render(ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.DynamicallyAllocatedBuffer<char> __buffer)")
            .OpenBracesBlock()
            .Append("const string __template =").AppendLineCore();

        var detentCount = 0;
        for(; detentCount < sourceBuilder.OpenBlocks; detentCount++)
            sourceBuilder.DetentCore();

        sourceBuilder
            .AppendLine("\"\"\"")
            .AppendLine(template.Source.Text)
            .AppendCore("\"\"\";");

        for(; detentCount > 0; detentCount--)
            sourceBuilder.IndentCore();

        sourceBuilder.AppendLine()
            .Append("System.ReadOnlySpan<char> __templateSpan = __template.AsSpan();").AppendLineCore();

        foreach(var templateChild in template.Children)
        {
            if(templateChild is TextSyntaxModel)
            {
                sourceBuilder
                    .Append("__buffer.Add(__templateSpan.Slice(")
                    .Append(templateChild.Source.Start.ToString()).Append(", ").Append(templateChild.Source.Length.ToString())
                    .Append("));").AppendLineCore();
            } else if(templateChild is ValueSyntaxModel)
            {
                var variableName = GetValueString(templateChild);

                sourceBuilder
                    .Append("global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(")
                    .Append(variableName).Append(", ref __buffer);").AppendLineCore();
            } else if(templateChild is CodeSyntaxModel code)
            {
                foreach(var codeChild in code.Children)
                {
                    if(codeChild is TextSyntaxModel)
                    {
                        sourceBuilder.AppendCore(codeChild.Source.ToString());
                    } else if(codeChild is ValueSyntaxModel)
                    {
                        var variableName = GetValueString(codeChild);

                        sourceBuilder
                            .Append("global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(")
                            .Append(variableName).AppendCore(", ref __buffer);");
                    }
                }
            }
        }

        var source = sourceBuilder.CloseAllBlocks().ToString();

        return (hintName, source);
    }

    private static unsafe String GetValueString(TemplateChildSyntaxModel model)
    {
        var span = model.Source.AsSpan[2..^1];

        fixed(Char* chars = span)
            return new(chars, 0, span.Length);
    }
}
