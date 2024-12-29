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
                .AppendLine("var buffer = new global::RhoMicro.CodeAnalysis.Library.Text.Templating.DynamicallyAllocatedBuffer<char>();")
                .Append("try")
                .OpenBracesBlock()
                    .AppendLine("this.Render(ref buffer);")
                    .AppendLine("fixed(char* chars = buffer.Span)")
                    .Indent().AppendLine("return new(chars, 0, buffer.Span.Length);").Detent()
                .CloseBlock()
                .Append("finally")
                .OpenBracesBlock()
                    .Append("buffer.Dispose();")
                .CloseBlock()
                .CloseBlockCore();
        }

        sourceBuilder
            .Append("public void Render(ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.DynamicallyAllocatedBuffer<char> __buffer, global::System.ReadOnlySpan<Char> __indentation, global::System.Threading.CancellationToken __cancellationToken)")
            .OpenBracesBlock()
            .AppendLine("__cancellationToken.ThrowIfCancellationRequested();")
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

        _ = sourceBuilder.AppendLine()
            .AppendLine("var __templateSpan = __template.AsSpan();")
            .OpenBracesBlock();

        foreach(var templateChild in template.Children)
        {
            if(templateChild is TextSyntaxModel)
            {
                sourceBuilder
                    .Append("global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(__templateSpan.Slice(")
                    .Append(templateChild.Source.Start.ToString()).Append(", ").Append(templateChild.Source.Length.ToString())
                    .Append("), ref __buffer, __indentation, __cancellationToken);").AppendLineCore();
            } else if(templateChild is ValueSyntaxModel)
            {
                var valueExpression = GetValueString(templateChild);

                sourceBuilder
                    .Append("global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(")
                    .Append(valueExpression).Append(", ref __buffer, __indentation, __cancellationToken);").AppendLineCore();
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

//[Template(
//"""

//""")]
//internal partial record Template(NamedTypeModel Type, TemplateSyntaxModel Template, TemplateAttribute.Model Attribute);

//[Template(
//"""
//<auto-generated/>
//#pragma warning disable
//#nullable enable

//namespace §{
//    for(var i = 0; i < Model.NamespaceParts.Count; i++)
//    {
//        if(i != 0)
//            §('.')

//        §(Model.NamespaceParts[i])
//    }
//};

////TODO: containing type
//partial §(Model.Kind.Value) §(Model.Name)§{
//    if(Model.TypeArguments.Count > 0)
//    {
//        §('<')

//        for(var i = 0; i < Model.TypeArguments.Count; i++)
//        {
//            §(Model.TypeArguments[i])
//        }

//        §('>')
//    }
//}{
//§(Body)
//}
//""")]
//internal partial record struct NamedTypeTemplate<TBody>(NamedTypeModel Model, TBody Body)
//    where TBody : ITemplate;

//[Template(
//"""
//partial §(model.TypeModifier)§(Params)
//{
//    §(body)
//}
//""")]
/*
[NonEquatable]
internal readonly partial struct ContainingType<TBody>(ContainingTypeModel model, TBody body)
    where TBody : ITemplate
{
    private TypeParametersTemplate Params => new(model.TypeParameters);
    public void Render(ref global::RhoMicro.CodeAnalysis.Library.Text.Templating.DynamicallyAllocatedBuffer<char> __buffer, ReadOnlySpan<Char> __indentation)
    {
        const string __template =
"""
partial §(model.TypeModifier)§(Params)
{
§(" ", body)
}
""";

        global::System.ReadOnlySpan<char> __templateSpan = __template.AsSpan();

        if(__indentation.Length == 0)
        {
            __buffer.Add(__templateSpan.Slice(0, 8));
            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(model.TypeModifier, ref __buffer);
            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(Params, ref __buffer);
            __buffer.Add(__templateSpan.Slice(38, 9));

            var __indentation1Span = __templateSpan.Slice(41, 4);
            Span<Char> __indentation1 = stackalloc Char[__indentation.Length + 4];
            __indentation.CopyTo(__indentation1);
            __indentation1Span.CopyTo(__indentation1[__indentation.Length..]);

            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(body, ref __buffer, __indentation1);

            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(body, ref __buffer);
            __buffer.Add(__templateSpan.Slice(54, 3));
        } else
        {
            // no nl
            __buffer.Add(__templateSpan.Slice(0, 8));
            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(model.TypeModifier, ref __buffer);
            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(Params, ref __buffer);
            // nl at index 38
            __buffer.Add(__templateSpan.Slice(38, 1));
            __buffer.Add(__indentation);
            // {nl at index 39
            __buffer.Add(__templateSpan.Slice(39, 2));
            __buffer.Add(__indentation);
            // spspspsp at index 42
            var __indentation1Span = __templateSpan.Slice(41, 4);
            __buffer.Add(__indentation1Span);

            Span<Char> __indentation1 = stackalloc Char[__indentation.Length + 4];
            __indentation.CopyTo(__indentation1);
            __indentation1Span.CopyTo(__indentation1[__indentation.Length..]);

            global::RhoMicro.CodeAnalysis.Library.Text.Templating.TemplateRuntimeHelpers.Render(body, ref __buffer, __indentation1);

            __buffer.Add(__templateSpan.Slice(54, 3));
        }
    }
}
*/

//[Template(
//"""
//§{
//if(typeParameters.Count > 0)
//{
//    §('<')

//    for(var i = 0; i < typeParameters.Count; i++)
//    {
//        §(typeParameters[i])
//    }

//    §('>')
//}
//}
//""")]
//[NonEquatable]
//internal readonly partial struct TypeParametersTemplate(EquatableList<String> typeParameters);