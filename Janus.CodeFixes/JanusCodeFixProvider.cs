using System;

namespace RhoMicro.CodeAnalysis.Janus;

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides code fixes for the Janus analyzer.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JanusCodeFixProvider)), Shared]
public class JanusCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public sealed override ImmutableArray<String> FixableDiagnosticIds { get; } =
    [
        DiagnosticIds.ClassUnionsShouldBeSealed
    ];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider()
        => FixAllProvider.Create(async (context, document, diagnostics) =>
        {
            var ct = context.CancellationToken;

            ct.ThrowIfCancellationRequested();

            if (diagnostics.IsEmpty)
            {
                return null;
            }

            return await GetTransformedDocumentAsync(
                    document,
                    diagnostics,
                    context.CancellationToken)
                .ConfigureAwait(false);
        });

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var ct = context.CancellationToken;

        ct.ThrowIfCancellationRequested();

        foreach (var diagnostic in context.Diagnostics)
        {
            ct.ThrowIfCancellationRequested();

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Seal union type",
                    cancellationToken => GetTransformedDocumentAsync(context.Document, [diagnostic], cancellationToken),
                    equivalenceKey: nameof(JanusCodeFixProvider)),
                diagnostic);
        }

        return Task.CompletedTask;
    }

    private static async Task<Document> GetTransformedDocumentAsync(
        Document document,
        ImmutableArray<Diagnostic> diagnosticsToFix,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var syntaxRoot = await document.GetSyntaxRootAsync(ct);

        if (syntaxRoot is null)
        {
            return document;
        }

        var nodesToReplace = diagnosticsToFix
            .Select(d =>
            {
                var result = syntaxRoot.FindNode(d.Location.SourceSpan) is TypeDeclarationSyntax s
                    ? s
                    : null;

                return result;
            })
            .OfType<TypeDeclarationSyntax>();

        var transformedSyntaxRoot = syntaxRoot.ReplaceNodes(
            nodesToReplace,
            (_, t) =>
            {
                if (t.Modifiers.Any(SyntaxKind.SealedKeyword))
                {
                    return t;
                }

                SyntaxTokenList modifiedModifiers;
                var sealedToken = SyntaxFactory.Token(SyntaxKind.SealedKeyword);
                if (t.Modifiers.FirstOrDefault(st => st.IsKind(SyntaxKind.PartialKeyword)) is
                    {
                        RawKind: (Int32)SyntaxKind.PartialKeyword
                    } partialToken)
                {
                    modifiedModifiers = t.Modifiers.ReplaceRange(partialToken, [sealedToken, partialToken]);
                }
                else
                {
                    modifiedModifiers = t.Modifiers.Add(sealedToken);
                }

                var modifiedNode = t.WithModifiers(modifiedModifiers);

                return modifiedNode;
            });
        
        var transformedDocument = document.WithSyntaxRoot(transformedSyntaxRoot);

        return transformedDocument;
    }
}
