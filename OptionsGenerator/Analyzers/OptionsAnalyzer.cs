namespace RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Provides diagnostics for the options generator.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionsAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the diagnostic descriptor for rule <c>ROG0001</c>.
    /// </summary>
    public static DiagnosticDescriptor TargetInterfacesCannotBeGeneric { get; } =
        new(
            id: DiagnosticIds.ROG0001TargetInterfacesCannotBeGeneric,
            title: "Target interfaces cannot be generic",
            messageFormat: "Target interface '{0}' cannot be generic",
            category: "OptionsGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Target interfaces cannot be generic."
        );

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        TargetInterfacesCannotBeGeneric
    ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        if(ctx.Symbol is not INamedTypeSymbol
           {
               TypeKind: TypeKind.Interface,
               Locations: [_, ..]
           } typeSymbol || !typeSymbol.GetAttributes().Any(a => a.IsOptionsAttribute()))
        {
            return;
        }

        if(typeSymbol.TypeParameters.Length > 0)
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    TargetInterfacesCannotBeGeneric,
                    typeSymbol.Locations[0],
                    typeSymbol.Name));
        }
    }
}