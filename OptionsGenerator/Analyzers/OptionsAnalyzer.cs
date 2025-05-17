namespace RhoMicro.CodeAnalysis.OptionsGenerator.Analyzers;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

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
    public static DiagnosticDescriptor OptionInterfacesCannotBeGeneric { get; } =
        new(
            id: DiagnosticIds.ROG0001OptionInterfacesCannotBeGeneric,
            title: "Option interfaces cannot be generic",
            messageFormat: "Option interface '{0}' cannot be generic",
            category: "OptionsGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Option interfaces cannot be generic."
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

    /// <summary>
    /// Gets the diagnostic descriptor for rule <c>ROG0003</c>.
    /// </summary>
    public static DiagnosticDescriptor OptionInterfacesCannotBeNested { get; } =
        new(
            id: DiagnosticIds.ROG0003OptionInterfacesCannotBeNested,
            title: "Option interfaces cannot be nested",
            messageFormat: "Option interface '{0}' cannot be nested",
            category: "OptionsGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Option interfaces cannot be nested."
        );

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        OptionInterfacesCannotBeGeneric,
        OptionsPropertiesMustBeReadOnly,
        OptionInterfacesCannotBeNested,
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

        if(!IsTargetProperty(ctx.Symbol, out var p) ||
           !p.ContainingType.GetAttributes().Any(a => a.IsOptionsAttribute()))
        {
            return false;
        }

        if(!IsReadOnlyProperty(p))
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

        if(!IsTargetInterface(ctx.Symbol, out var i) || !i.GetAttributes().Any(a => a.IsOptionsAttribute()))
        {
            return false;
        }

        if(!IsNonGenericType(i))
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    OptionInterfacesCannotBeGeneric,
                    i.Locations[0],
                    i.ToDisplayString(_symbolDisplayFormat)));
        }

        if(!IsTopLevelType(i))
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    OptionInterfacesCannotBeNested,
                    i.Locations[0],
                    i.ToDisplayString(_symbolDisplayFormat)));
        }

        return true;
    }

    internal static Boolean IsValidTargetInterface(INamedTypeSymbol target)
        => IsNonGenericType(target) && IsTopLevelType(target);

    private static Boolean IsNonGenericType(INamedTypeSymbol target) => target.TypeParameters is [];
    private static Boolean IsTopLevelType(INamedTypeSymbol target) => target.ContainingType is null;

    internal static Boolean IsTargetInterface(ISymbol symbol, [NotNullWhen(true)] out INamedTypeSymbol? target)
    {
        if(symbol is INamedTypeSymbol
           {
               TypeKind: TypeKind.Interface,
               Locations: [_, ..]
           } i)
        {
            target = i;
            return true;
        }

        target = null;
        return false;
    }

    internal static Boolean IsValidTargetProperty(IPropertySymbol target) => IsReadOnlyProperty(target);
    private static Boolean IsReadOnlyProperty(IPropertySymbol target) => target.SetMethod is null;

    internal static Boolean IsTargetProperty(ISymbol symbol, [NotNullWhen(true)] out IPropertySymbol? target)
    {
        if(symbol is IPropertySymbol
           {
               DeclaringSyntaxReferences: [_, ..]
           } p &&
           !p.GetAttributes().Any(a => a.IsExcludeFromOptionsAttribute()))
        {
            target = p;
            return true;
        }

        target = null;
        return false;
    }
}