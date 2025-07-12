// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Linq;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;

using Diagnostic = CodeAnalysis.Templating.Diagnostic;

public partial class ParserTests(ITestOutputHelper testOutput)
{
    private void TestParser(
        String sourceText,
        Func<TokenList, TemplateSyntax>? syntaxFactory = null,
        Func<TokenList, EquatableList<Diagnostic>>? diagnosticsFactory = null,
        Int32 newlineLength = 1)
    {
        // Arrange
        var token = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree
            .ParseText(sourceText)
            .GetRoot()
            .DescendantTokens(_ => true)
            .Single(t => t.RawKind is
                (Int32)Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralToken or
                (Int32)Microsoft.CodeAnalysis.CSharp.SyntaxKind.MultiLineRawStringLiteralToken or
                (Int32)Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineRawStringLiteralToken);

        var templateString = TemplateString.Create(token, TestContext.Current.CancellationToken);
        ScanResult scanResult;

        using(var ctx = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
            scanResult = Lexer.Scan(templateString, newlineLength, in ctx);

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
            var left = expectedSyntax.ToXmlTreeString(TestContext.Current.CancellationToken);

            if(!expectedSyntax.Equals(actualSyntax))
            {
                TokenAssertionVisitor.Verify(actualSyntax, "actual syntax was malformed");

                var right = actualSyntax.ToXmlTreeString(TestContext.Current.CancellationToken);
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
        new TemplateSyntax(
            new NotEmptyTemplateSyntax(
                new TemplateBlockBodySyntax([
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next())),
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))]))])),
                    new TemplateBlockBodyChildSyntax(
                        new CodeBlockSyntax(
                            null,
                            new OpenCodeBlockSyntax(t.Next()),
                            new CodeBlockBodySyntax([
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                    new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new WhitespacesSyntax(t.Next()))]))])),
                                new CodeBlockBodyChildSyntax(
                                    new RenderBlockSyntax(
                                    new RenderBlockHeadSyntax(
                                        new OpenRenderBlockSyntax(t.Next()),
                                        new TextSyntax([
                                            new TextChildSyntax(
                                                new NotEscapedTextSyntax([
                                                    new NotEscapedTextChildSyntax(
                                                        new NotNewlineSyntax(t.Next()))]))]),
                                        new CloseRenderBlockSyntax(t.Next())),
                                    null)),
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                    new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next()))]))]))]),
                            new CloseCodeBlockSyntax(t.Next()),
                            null))]))));
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
        new TemplateSyntax(
            new NotEmptyTemplateSyntax(
                new TemplateBlockBodySyntax([
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next())),
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))]))])),
                    new TemplateBlockBodyChildSyntax(
                        new CodeBlockSyntax(
                            null,
                            new OpenCodeBlockSyntax(t.Next()),
                            new CodeBlockBodySyntax([
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new WhitespacesSyntax(t.Next()))]))])),
                                new CodeBlockBodyChildSyntax(
                                    new RenderBlockSyntax(
                                    new RenderBlockHeadSyntax(
                                        new OpenRenderBlockSyntax(t.Next()),
                                        new TextSyntax([
                                            new TextChildSyntax(
                                                new NotEscapedTextSyntax([
                                                    new NotEscapedTextChildSyntax(
                                                        new NotNewlineSyntax(t.Next()))]))]),
                                        new CloseRenderBlockSyntax(t.Next())),
                                    new RenderBlockBodySyntax(
                                        new RenderBlockTriviaSyntax(
                                            new NewlineSyntax(t.Next())),
                                        new TemplateBlockSyntax(
                                            new LeadingTriviaSyntax(
                                                new WhitespacesSyntax(t.Next())),
                                            new OpenTemplateBlockSyntax(t.Next()),
                                            new TemplateBlockBodySyntax([
                                                new TemplateBlockBodyChildSyntax(
                                                    new TextSyntax([
                                                        new TextChildSyntax(
                                                            new NotEscapedTextSyntax([
                                                                new NotEscapedTextChildSyntax(new NewlineSyntax(t.Next())),
                                                                new NotEscapedTextChildSyntax(new NotNewlineSyntax(t.Next()))]))])),
                                                new TemplateBlockBodyChildSyntax(
                                                    new RenderBlockSyntax(
                                                        new RenderBlockHeadSyntax(
                                                            new OpenRenderBlockSyntax(t.Next()),
                                                            new TextSyntax([
                                                                new TextChildSyntax(
                                                                    new NotEscapedTextSyntax([
                                                                        new NotEscapedTextChildSyntax(
                                                                            new NotNewlineSyntax(t.Next()))]))]),
                                                            new CloseRenderBlockSyntax(t.Next())),
                                                        null)),
                                                new TemplateBlockBodyChildSyntax(
                                                    new TextSyntax([
                                                        new TextChildSyntax(
                                                            new NotEscapedTextSyntax([
                                                                new NotEscapedTextChildSyntax(
                                                                    new NotNewlineSyntax(t.Next())),
                                                                new NotEscapedTextChildSyntax(
                                                                    new NewlineSyntax(t.Next())),
                                                                new NotEscapedTextChildSyntax(
                                                                    new WhitespacesSyntax(t.Next()))]))]))]),
                                            new CloseTemplateBlockSyntax(t.Next()),
                                            new TrailingTriviaSyntax(
                                                new NewlineSyntax(t.Next())))))),
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next()))]))]))]),
                            new CloseCodeBlockSyntax(t.Next()),
                            null))]))));
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
        new TemplateSyntax(
            new NotEmptyTemplateSyntax(
                new TemplateBlockBodySyntax([
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next())),
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))]))])),
                    new TemplateBlockBodyChildSyntax(
                        new CodeBlockSyntax(
                            null,
                            new OpenCodeBlockSyntax(t.Next()),
                            new CodeBlockBodySyntax([
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new WhitespacesSyntax(t.Next()))])),
                                        new TextChildSyntax(
                                        new EscapedTextSyntax(
                                            new EscapedOpenBlockSyntax(
                                                new OpenBlockSyntax(
                                                    new OpenRenderBlockSyntax(t.Next())),
                                                new EscapeColonSyntax(t.Next())))),
                                        new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next()))])),
                                        new TextChildSyntax(
                                        new EscapedTextSyntax(
                                            new EscapedCloseBlockSyntax(
                                                new EscapeColonSyntax(t.Next()),
                                                new CloseBlockSyntax(
                                                    new CloseRenderBlockSyntax(t.Next()))))),
                                        new TextChildSyntax(
                                        new EscapedTextSyntax(
                                            new EscapedOpenBlockSyntax(
                                                new OpenBlockSyntax(
                                                    new OpenTemplateBlockSyntax(t.Next())),
                                                new EscapeColonSyntax(t.Next())))),
                                        new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NewlineSyntax(t.Next())),
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next()))]))])),
                                new CodeBlockBodyChildSyntax(
                                    new RenderBlockSyntax(
                                        new RenderBlockHeadSyntax(
                                            new OpenRenderBlockSyntax(t.Next()),
                                            new TextSyntax([
                                                new TextChildSyntax(
                                                    new NotEscapedTextSyntax([
                                                        new NotEscapedTextChildSyntax(
                                                            new NotNewlineSyntax(t.Next()))]))]),
                                            new CloseRenderBlockSyntax(t.Next())),
                                        null)),
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new WhitespacesSyntax(t.Next()))])),
                                        new TextChildSyntax(
                                            new EscapedTextSyntax(
                                                new EscapedCloseBlockSyntax(
                                                    new EscapeColonSyntax(t.Next()),
                                                    new CloseBlockSyntax(
                                                        new CloseTemplateBlockSyntax(t.Next()))))),
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next()))]))]))]),
                            new CloseCodeBlockSyntax(t.Next()),
                            new TrailingTriviaSyntax(
                                new NewlineSyntax(t.Next())))),
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next()))]))]))]))));

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
        new TemplateSyntax(
            new NotEmptyTemplateSyntax(
                new TemplateBlockBodySyntax([
                    new TemplateBlockBodyChildSyntax(
                        new CodeBlockSyntax(
                            null,
                            new OpenCodeBlockSyntax(t.Next()),
                            new CodeBlockBodySyntax([
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next()))]))]))]),
                            new CloseCodeBlockSyntax(t.Next()),
                            new TrailingTriviaSyntax(
                                new NewlineSyntax(t.Next())))),
                    new TemplateBlockBodyChildSyntax(
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next()))]))]),
                                new CloseRenderBlockSyntax(t.Next())),
                            null)),
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))]))])),
                    new TemplateBlockBodyChildSyntax(
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next()))]))]),
                                new CloseRenderBlockSyntax(t.Next())),
                            null)),
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))]))])),
                    new TemplateBlockBodyChildSyntax(
                        new RenderBlockSyntax(
                            new RenderBlockHeadSyntax(
                                new OpenRenderBlockSyntax(t.Next()),
                                new TextSyntax([
                                    new TextChildSyntax(
                                        new NotEscapedTextSyntax([
                                            new NotEscapedTextChildSyntax(
                                                new NotNewlineSyntax(t.Next()))]))]),
                                new CloseRenderBlockSyntax(t.Next())),
                            new RenderBlockBodySyntax(
                                null,
                                new TemplateBlockSyntax(
                                    null,
                                    new OpenTemplateBlockSyntax(t.Next()),
                                    new TemplateBlockBodySyntax([
                                        new TemplateBlockBodyChildSyntax(
                                            new TextSyntax([
                                                new TextChildSyntax(
                                                    new NotEscapedTextSyntax([
                                                        new NotEscapedTextChildSyntax(
                                                            new NewlineSyntax(t.Next())),
                                                        new NotEscapedTextChildSyntax(
                                                            new NotNewlineSyntax(t.Next()))]))])),
                                        new TemplateBlockBodyChildSyntax(
                                            new RenderBlockSyntax(
                                                new RenderBlockHeadSyntax(
                                                    new OpenRenderBlockSyntax(t.Next()),
                                                    new TextSyntax([
                                                        new TextChildSyntax(
                                                            new NotEscapedTextSyntax([
                                                                new NotEscapedTextChildSyntax(
                                                                    new NotNewlineSyntax(t.Next()))]))]),
                                                    new CloseRenderBlockSyntax(t.Next())),
                                                null)),
                                        new TemplateBlockBodyChildSyntax(
                                            new TextSyntax([
                                                new TextChildSyntax(
                                                    new NotEscapedTextSyntax([
                                                        new NotEscapedTextChildSyntax(
                                                            new NotNewlineSyntax(t.Next()))]))])),
                                        new TemplateBlockBodyChildSyntax(
                                            new RenderBlockSyntax(
                                                new RenderBlockHeadSyntax(
                                                    new OpenRenderBlockSyntax(t.Next()),
                                                    new TextSyntax([
                                                        new TextChildSyntax(
                                                            new NotEscapedTextSyntax([
                                                                new NotEscapedTextChildSyntax(
                                                                    new NotNewlineSyntax(t.Next()))]))]),
                                                    new CloseRenderBlockSyntax(t.Next())),
                                                null)),
                                        new TemplateBlockBodyChildSyntax(
                                            new TextSyntax([
                                                new TextChildSyntax(
                                                    new NotEscapedTextSyntax([
                                                        new NotEscapedTextChildSyntax(
                                                            new NotNewlineSyntax(t.Next())),
                                                        new NotEscapedTextChildSyntax(
                                                            new NewlineSyntax(t.Next()))]))]))]),
                                    new CloseTemplateBlockSyntax(t.Next()),
                                    null))))]))));
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
        new TemplateSyntax(
            new NotEmptyTemplateSyntax(
                new TemplateBlockBodySyntax([
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next())),
                                    new NotEscapedTextChildSyntax(
                                        new NewlineSyntax(t.Next()))
                                    ]))])),
                    new TemplateBlockBodyChildSyntax(
                        new CodeBlockSyntax(
                            null,
                            new OpenCodeBlockSyntax(t.Next()),
                            new CodeBlockBodySyntax([
                                new CodeBlockBodyChildSyntax(
                                    new TextSyntax([
                                        new TextChildSyntax(
                                            new NotEscapedTextSyntax([
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NotNewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new NewlineSyntax(t.Next())),
                                                new NotEscapedTextChildSyntax(
                                                    new WhitespacesSyntax(t.Next()))]))])),
                                    new CodeBlockBodyChildSyntax(
                                        new RenderBlockSyntax(
                                            new RenderBlockHeadSyntax(
                                                new OpenRenderBlockSyntax(t.Next()),
                                                new TextSyntax([
                                                    new TextChildSyntax(
                                                        new NotEscapedTextSyntax([
                                                            new NotEscapedTextChildSyntax(
                                                                new NotNewlineSyntax(t.Next()))]))]),
                                                new CloseRenderBlockSyntax(t.Next())),
                                            null)),
                                    new CodeBlockBodyChildSyntax(
                                        new TextSyntax([
                                            new TextChildSyntax(
                                                new EscapedTextSyntax(
                                                    new EscapedOpenBlockSyntax(
                                                        new OpenBlockSyntax(
                                                            new OpenTemplateBlockSyntax(t.Next())),
                                                        new EscapeColonSyntax(t.Next())))),
                                            new TextChildSyntax(
                                                new NotEscapedTextSyntax([
                                                    new NotEscapedTextChildSyntax(
                                                        new NewlineSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new NotNewlineSyntax(t.Next()))]))])),
                                    new CodeBlockBodyChildSyntax(
                                        new RenderBlockSyntax(
                                            new RenderBlockHeadSyntax(
                                                new OpenRenderBlockSyntax(t.Next()),
                                                new TextSyntax([
                                                    new TextChildSyntax(
                                                        new NotEscapedTextSyntax([
                                                            new NotEscapedTextChildSyntax(
                                                                new NotNewlineSyntax(t.Next()))]))]),
                                                new CloseRenderBlockSyntax(t.Next())),
                                            null)),
                                    new CodeBlockBodyChildSyntax(
                                        new TextSyntax([
                                            new TextChildSyntax(
                                                new NotEscapedTextSyntax([
                                                    new NotEscapedTextChildSyntax(
                                                        new NotNewlineSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new NewlineSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new WhitespacesSyntax(t.Next()))])),
                                            new TextChildSyntax(
                                                new EscapedTextSyntax(
                                                    new EscapedCloseBlockSyntax(
                                                        new EscapeColonSyntax(t.Next()),
                                                        new CloseBlockSyntax(
                                                            new CloseTemplateBlockSyntax(t.Next()))))),
                                            new TextChildSyntax(
                                                new NotEscapedTextSyntax([
                                                    new NotEscapedTextChildSyntax(
                                                        new WhitespacesSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new NewlineSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new NotNewlineSyntax(t.Next())),
                                                    new NotEscapedTextChildSyntax(
                                                        new NewlineSyntax(t.Next()))]))]))]),
                            new CloseCodeBlockSyntax(t.Next()),
                            new TrailingTriviaSyntax(
                                new NewlineSyntax(t.Next())))),
                    new TemplateBlockBodyChildSyntax(
                        new TextSyntax([
                            new TextChildSyntax(
                                new NotEscapedTextSyntax([
                                    new NotEscapedTextChildSyntax(
                                        new NotNewlineSyntax(t.Next()))]))]))]))));
    [Theory]
    [InlineData(
        """
        ""
        """)]
    [InlineData(
        """
        "Hello, World!"
        """)]
    [InlineData(
        """
        "(:Foo:)"
        """)]
    [InlineData(
        """
        "Lorem ipsum dolor sit amet"
        """)]
    [InlineData(
        """
        "🚀🌟💻🎉"
        """)]
    [InlineData(
        """
        "This is a longer string to test"
        """)]
    [InlineData(
        """
        "1234567890"
        """)]
    [InlineData(
        """
        "Special characters: !@#$%^&*()_+-=[]{}|;:'\",.<>?/\\"
        """)]
    [InlineData(
        """
        "Single space "
        """)]
    [InlineData(
        """
        " Leading space"
        """)]
    [InlineData(
        """
        "Line\nBreak"
        """)]
    [InlineData(
        """
        "Tab\tCharacter"
        """)]
    [InlineData(
        """
        "NullChar\u0000Here"
        """)]
    [InlineData(
        """
        "こんにちは"
        """)]
    [InlineData(
        """
        "你好"
        """)]
    [InlineData(
        """
        "안녕하세요"
        """)]
    [InlineData(
        """
        "Привет"
        """)]
    [InlineData(
        """
        "مرحبا"
        """)]
    [InlineData(
        """
        "String with an emoji 🤖 at the end"
        """)]
    [InlineData(
        """
        "Repeat: Repeat: Repeat: Repeat:"
        """)]
    [InlineData(
        """"
        """
        First text
        (:Foo:)
        Second Text
        """
        """")]
    [InlineData(
        """"
        """
        First text
        {:
            for(var i = 0; i < 5; i++)
                (:Foo:)
        :}
        Second Text
        """
        """")]
    [InlineData(
        """"
        """
        Prefix
        {:
            for(var i = 0; i < count; i++)
                (:child:)
        :}
        Suffix
        """
        """")]
    [InlineData(
        """"
        "This (:Value:) should not be rendered via (:nameof(ToString):)."
        """")]
    [InlineData(
        """"
        """
        Dear (:Salutation:),

        {:
            foreach (var name in Names)
            {
                (:"  Hello ":)(:name:)(:", hope you are doing well!\n":)
            }
        :}

        It's always great to stay in touch with everyone!

        Best regards,
        (:Closing:)
        """
        """")]
    [InlineData(
        """"
        """
        {:
            (:Tab, lines:)
        :}
        """
        """")]
    [InlineData(
        """"
        """
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
            return;
        }
        """
        """")]
    [InlineData(
        """"
        """
        public class (:name:)
        {
        (:Tab, body:)
        }
        """
        """")]
    [InlineData(
        """"
        """
        <div>
        (:Tab, Body:)
        </div>
        """
        """")]
    [InlineData(
        """
        {:var foo = "foo";:}
        (:new bartemplate():)

        (:new layouttemplate():)
        <:
        <h1>(:greeting:), (:foo:)!</h1>
        :>
        """)]
    [InlineData(
        """
        "Bar"
        """)]
    public void DoesNotThrow(String sourceText) => TestParser(sourceText);
}
