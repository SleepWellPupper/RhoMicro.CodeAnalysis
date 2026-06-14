using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace NoBang;

/// <summary>
/// Reports use of the null forgiving operator.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Analyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            ctx =>
            {
                ctx.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.TheNullForgivingOperatorIsACodeSmell,
                        ctx.Node.GetLocation()));
            },
            SyntaxKind.SuppressNullableWarningExpression);
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = [DiagnosticDescriptors.TheNullForgivingOperatorIsACodeSmell];
}
