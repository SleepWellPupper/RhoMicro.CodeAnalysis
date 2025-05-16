namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Generated;
using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

/// <summary>
/// Source generator for generating option pattern implementations from interfaces.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class OptionsGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(OptionsAttribute).FullName,
                static (n, _) => n is InterfaceDeclarationSyntax
                {
                    TypeParameterList: null
                },
                static (ctx, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    if(ctx is not
                        {
                            TargetSymbol: INamedTypeSymbol
                            {
                                TypeKind: TypeKind.Interface,
                                TypeParameters: []
                            } type,
                            Attributes: [{ } a, ..]
                        } || !a.TryGetOptionsAttributeModel(out var attribute, cancellationToken: ct))
                    {
                        return null;
                    }

                    using var modelContext = ModelCreationContext.CreateDefault(ct);

                    if(!OptionsModel.TryCreate(type, attribute, out var result, in modelContext))
                        return null;

                    return result;
                }).Where(m => m is not null)
                .Select((m, ct) =>
                {
                    var model = m!;
                    var hintName = $"{model.NamespacePrefix}{model.Name}.g.cs";
                    var source = new GeneratedFileTemplate(
                        name: nameof(OptionsGenerator))
                        .RenderToString(model.Templates().Root, ct);

                    return (hintName, source);
                });

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));

        IncludedFileSources.RegisterToContext(context);
    }
}
