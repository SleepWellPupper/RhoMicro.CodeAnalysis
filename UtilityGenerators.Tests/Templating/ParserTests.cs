#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;
using DiffPlex;
using System.Numerics;
using Xunit.Sdk;
using RhoMicro.CodeAnalysis.Templating.Syntax;

public partial class ParserTests(ITestOutputHelper testOutput)
{
    private void TestParser(
        String sourceText,
        Func<TokenList, TemplateSyntax>? syntaxFactory = null,
        Func<TokenList, EquatableList<Diagnostic>>? diagnosticsFactory = null)
    {
        // Arrange
        var token = CSharpSyntaxTree
            .ParseText(sourceText)
            .GetRoot()
            .DescendantTokens(_ => true)
            .Single(t => t.RawKind is
                (Int32)SyntaxKind.StringLiteralToken or
                (Int32)SyntaxKind.MultiLineRawStringLiteralToken or
                (Int32)SyntaxKind.SingleLineRawStringLiteralToken);

        var templateString = TemplateString.Create(token, TestContext.Current.CancellationToken);
        ScanResult scanResult;

        using(var ctx = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
            scanResult = Lexer.Scan(templateString, in ctx);

        var nullableExpectedSyntax = syntaxFactory?.Invoke(new(scanResult.Tokens));
        if(nullableExpectedSyntax is { } s)
        {
            TokenAssertionVisitor.Verify(s, "expected syntax was malformed");
        }

        var nullableExpectedDiagnostics = diagnosticsFactory?.Invoke(new(scanResult.Tokens));

        TemplateSyntax actualSyntax;
        EquatableList<Diagnostic> actualDiagnostics;

        // Act
        using(var context = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
            (actualSyntax, _, actualDiagnostics) = Parser.Parse(scanResult, in context);

        // Assert
        if(nullableExpectedSyntax is { } expectedSyntax)
        {
            var left = expectedSyntax.ToAstString();

            if(!expectedSyntax.Equals(actualSyntax))
            {
                TokenAssertionVisitor.Verify(actualSyntax, "actual syntax was malformed");

                var right = actualSyntax.ToAstString();
                TestHelpers.FailWithDiff(left, right);
            }

            testOutput.WriteLine(left);
        }

        if(nullableExpectedDiagnostics is { } expectedDiagnostics
            && !expectedDiagnostics.Equals(actualDiagnostics))
        {
            var left = String.Join('\n', expectedDiagnostics);
            var right = String.Join('\n', actualDiagnostics);
            TestHelpers.FailWithDiff(left, right);
        }
    }

    [Fact]
    public void Parser_Parses_complex_1() => TestParser(
        """"
        """
        foo
        {:
            if(condition)
            {
                (:value:)
            }
        :}
        """
        """", t =>
        new NotEmptyTemplateSyntax(
            new TemplateBlockBodySyntax([
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())]),
                new BlockSequenceSyntax(
                    new CodeBlockSyntax(
                        new OpenCodeBlockSyntax(t.Next()),
                        new CodeBodySyntax([
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())]),
                            new RenderBlockSyntax(
                                new RenderBlockHeadSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new TextSyntax([
                                        new NotEscapedTextSyntax(t.Next())]),
                                    new CloseRenderBlockSyntax(t.Next()))),
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())])]),
                        new CloseCodeBlockSyntax(t.Next())))])));
    [Fact]
    public void Parser_Parses_complex_2() => TestParser(
        """"
        """
        foo
        {:
            if(condition)
            {
                var value2 = "World";
                (:value:)
                <:
                    Hello, (:value2:)!
                :>
            }
        :}
        """
        """", t =>
        new NotEmptyTemplateSyntax(
            new TemplateBlockBodySyntax([
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())]),
                new BlockSequenceSyntax(
                    new CodeBlockSyntax(
                        new OpenCodeBlockSyntax(t.Next()),
                        new CodeBodySyntax([
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())]),
                            new RenderBlockSyntax(
                                new RenderBlockHeadSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new TextSyntax([
                                        new NotEscapedTextSyntax(t.Next())]),
                                    new CloseRenderBlockSyntax(t.Next())),
                                new RenderBlockBodySyntax(
                                    new TriviaSyntax(t.Next()),
                                    new TemplateBlockSyntax(
                                        new OpenTemplateBlockSyntax(t.Next()),
                                        new TemplateBlockBodySyntax([
                                            new TextSyntax([
                                                new NotEscapedTextSyntax(t.Next())]),
                                            new BlockSequenceSyntax(
                                                new RenderBlockSyntax(
                                                    new RenderBlockHeadSyntax(
                                                        new OpenRenderBlockSyntax(t.Next()),
                                                        new TextSyntax([
                                                            new NotEscapedTextSyntax(t.Next())]),
                                                        new CloseRenderBlockSyntax(t.Next())))),
                                            new TextSyntax([
                                                new NotEscapedTextSyntax(t.Next())])]),
                                        new CloseTemplateBlockSyntax(t.Next())))),
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())])]),
                        new CloseCodeBlockSyntax(t.Next())))])));
    [Fact]
    public void Parser_Parses_complex_3() => TestParser(
        """"
        """
        foo
        {:
            if(condition)
            {
                var value2 = "World";
                (::value::)<::
                    Hello, (:value2:)!
                ::>
            }
        :}
        bar
        """
        """", t =>
        new NotEmptyTemplateSyntax(
            new TemplateBlockBodySyntax([
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())]),
                new BlockSequenceSyntax(
                    new CodeBlockSyntax(
                        new OpenCodeBlockSyntax(t.Next()),
                        new CodeBodySyntax([
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next()),
                                new EscapedOpenBlockSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new EscapeColonSyntax(t.Next())),
                                new NotEscapedTextSyntax(t.Next()),
                                new EscapedCloseBlockSyntax(
                                    new EscapeColonSyntax(t.Next()),
                                    new CloseRenderBlockSyntax(t.Next())),
                                new EscapedOpenBlockSyntax(
                                    new OpenTemplateBlockSyntax(t.Next()),
                                    new EscapeColonSyntax(t.Next())),
                                new NotEscapedTextSyntax(t.Next())]),
                            new RenderBlockSyntax(
                                new RenderBlockHeadSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new TextSyntax([
                                        new NotEscapedTextSyntax(t.Next())]),
                                    new CloseRenderBlockSyntax(t.Next()))),
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next()),
                                new EscapedCloseBlockSyntax(
                                    new EscapeColonSyntax(t.Next()),
                                    new CloseTemplateBlockSyntax(t.Next())),
                                new NotEscapedTextSyntax(t.Next())])]),
                        new CloseCodeBlockSyntax(t.Next()))),
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())])])));
    [Fact]
    public void Parser_Reports_UnexpectedToken1() => TestParser(
        """"
        """
        foo
        {:
            if(condition)
            {
                var value2 = "World";
                (::value:)<::
                    Hello, (:value2:)!
                ::>
            }
        :}
        bar
        """
        """",
        diagnosticsFactory: t =>
        [
            Diagnostic.Create(
                Diagnostic.Ids.UnexpectedToken,
                DiagnosticSeverity.Error,
                t.Next().TemplateString.Path,
                t.Next().Spans.SourceSpan)
        ]);
    [Fact]
    public void Parser_Parses_complex_escaped1() => TestParser(
        """"
        """
        {:var foo = "foo";:}
        (:new BarTemplate():)
        (:Greeting:)
        (:new LayoutTemplate():)<:
            <h1>(:Greeting:), (:foo:)!</h1>
        :>
        """
        """", t =>
        new NotEmptyTemplateSyntax(
            new TemplateBlockBodySyntax([
                new BlockSequenceSyntax(
                    new CodeBlockSyntax(
                        new OpenCodeBlockSyntax(t.Next()),
                        new CodeBodySyntax([
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())])]),
                        new CloseCodeBlockSyntax(t.Next())),[
                    new TriviaBlockSyntax(
                        new TriviaSyntax(t.Next()),
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new NotEscapedTextSyntax(t.Next())]),
                                new CloseRenderBlockSyntax(t.Next())))),
                    new TriviaBlockSyntax(
                        new TriviaSyntax(t.Next()),
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new NotEscapedTextSyntax(t.Next())]),
                                new CloseRenderBlockSyntax(t.Next())))),
                    new TriviaBlockSyntax(
                        new TriviaSyntax(t.Next()),
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new NotEscapedTextSyntax(t.Next())]),
                                new CloseRenderBlockSyntax(t.Next())),
                            new RenderBlockBodySyntax(
                                new TemplateBlockSyntax(
                                    new OpenTemplateBlockSyntax(t.Next()),
                                    new TemplateBlockBodySyntax([
                                        new TextSyntax([
                                            new NotEscapedTextSyntax(t.Next())]),
                                        new BlockSequenceSyntax(
                                            new RenderBlockSyntax(
                                                new RenderBlockHeadSyntax(
                                                    new OpenRenderBlockSyntax(t.Next()),
                                                    new TextSyntax([
                                                        new NotEscapedTextSyntax(t.Next())]),
                                                    new CloseRenderBlockSyntax(t.Next())))),
                                        new TextSyntax([
                                            new NotEscapedTextSyntax(t.Next())]),
                                        new BlockSequenceSyntax(
                                            new RenderBlockSyntax(
                                                new RenderBlockHeadSyntax(
                                                    new OpenRenderBlockSyntax(t.Next()),
                                                    new TextSyntax([
                                                        new NotEscapedTextSyntax(t.Next())]),
                                                    new CloseRenderBlockSyntax(t.Next())))),
                                        new TextSyntax([
                                            new NotEscapedTextSyntax(t.Next())])]),
                                    new CloseTemplateBlockSyntax(t.Next())))))])])));
    [Fact]
    public void Parser_Parses_complex_escaped2() => TestParser(
        """"
        """
        foo
        {:
            if(condition)
            {
                var value2 = "World";
                (:value:)<::
                    Hello, (:value2:)!
                ::>
            }
        :}
        bar
        """
        """", t =>
        new NotEmptyTemplateSyntax(
            new TemplateBlockBodySyntax([
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())]),
                new BlockSequenceSyntax(
                    new CodeBlockSyntax(
                        new OpenCodeBlockSyntax(t.Next()),
                        new CodeBodySyntax([
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next())]),
                            new RenderBlockSyntax(
                                new RenderBlockHeadSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new TextSyntax([
                                        new NotEscapedTextSyntax(t.Next())]),
                                    new CloseRenderBlockSyntax(t.Next()))),
                            new TextSyntax([
                                new EscapedOpenBlockSyntax(
                                    new OpenTemplateBlockSyntax(t.Next()),
                                    new EscapeColonSyntax(t.Next())),
                                new NotEscapedTextSyntax(t.Next())]),
                            new RenderBlockSyntax(
                                new RenderBlockHeadSyntax(
                                    new OpenRenderBlockSyntax(t.Next()),
                                    new TextSyntax([
                                        new NotEscapedTextSyntax(t.Next())]),
                                    new CloseRenderBlockSyntax(t.Next()))),
                            new TextSyntax([
                                new NotEscapedTextSyntax(t.Next()),
                                new EscapedCloseBlockSyntax(
                                    new EscapeColonSyntax(t.Next()),
                                    new CloseTemplateBlockSyntax(t.Next())),
                                new NotEscapedTextSyntax(t.Next())])]),
                        new CloseCodeBlockSyntax(t.Next()))),
                new TextSyntax([
                    new NotEscapedTextSyntax(t.Next())])])));
}
