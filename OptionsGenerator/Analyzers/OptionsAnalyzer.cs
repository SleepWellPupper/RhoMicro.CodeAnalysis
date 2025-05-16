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

    /// <summary>
    /// Gets the diagnostic descriptor for rule <c>ROG0002</c>.
    /// </summary>
    public static DiagnosticDescriptor OptionsPropertiesMustBeReadOnly { get; } =
        new(
            id: DiagnosticIds.ROG0002OptionsPropertiesMustBeReadOnly,
            title: "Options properties must be read only",
            messageFormat: "Option property {0} must be read only",
            category: "OptionsGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Options properties must be read only."
        );

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        TargetInterfacesCannotBeGeneric,
        OptionsPropertiesMustBeReadOnly
    ];

    private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
        propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor
    );
    
    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType, SymbolKind.Property);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext ctx)
    {
        if(AnalyzeNamedTypeSymbol(ctx))
            return;
        
        if(AnalyzePropertySymbol(ctx))
            return;
    }

    private static Boolean AnalyzePropertySymbol(SymbolAnalysisContext ctx)
    {
       ctx.CancellationToken.ThrowIfCancellationRequested();

       if(ctx.Symbol is not IPropertySymbol p || 
          p.GetAttributes().Any(a=>a.IsExcludeFromOptionsAttribute()) ||
          !p.ContainingType.GetAttributes().Any(a => a.IsOptionsAttribute()))
       {
           return false;
       }

       if(p.GetMethod is not null)
       {
           ctx.ReportDiagnostic(
               Diagnostic.Create(
                   OptionsPropertiesMustBeReadOnly,
                   p.Locations[0],
                   p.ToDisplayString(_symbolDisplayFormat)));
       }

       return true;
    }

private static Boolean AnalyzeNamedTypeSymbol(SymbolAnalysisContext ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        if(ctx.Symbol is not INamedTypeSymbol
           {
               TypeKind: TypeKind.Interface,
               Locations: [_, ..]
           } typeSymbol || !typeSymbol.GetAttributes().Any(a => a.IsOptionsAttribute()))
        {
            return false;
        }

        if(typeSymbol.TypeParameters.Length > 0)
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    TargetInterfacesCannotBeGeneric,
                    typeSymbol.Locations[0],
                    typeSymbol.ToDisplayString(_symbolDisplayFormat)));
        }

        return true;
    }
}