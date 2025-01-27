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

public class ParserTests
{
    private static void TestParser(
        String sourceText,
        Func<EquatableList<Token>, TemplateSyntax>? syntaxFactory = null,
        Func<EquatableList<Token>, EquatableList<Diagnostic>>? diagnosticsFactory = null)
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

        var templateString = TemplateString.Create(token, CancellationToken.None);
        ScanResult scanResult;

        using(var ctx = ModelCreationContext.CreateDefault(CancellationToken.None))
            scanResult = Lexer.Scan(templateString, in ctx);

        var nullableExpectedSyntax = syntaxFactory?.Invoke(scanResult.Tokens);
        if(nullableExpectedSyntax is { } s)
        {
            try
            {
                TokenAssertionVisitor.Instance.Verify(s);
            } catch(XunitException ex)
            {
                throw new InvalidOperationException("expected syntax was malformed.", ex);
            }
        }

        var nullableExpectedDiagnostics = diagnosticsFactory?.Invoke(scanResult.Tokens);

        TemplateSyntax actualSyntax;
        EquatableList<Diagnostic> actualDiagnostics;

        // Act
        using(var context = ModelCreationContext.CreateDefault(CancellationToken.None))
            (actualSyntax, _, actualDiagnostics) = Parser.Parse(scanResult, in context);

        // Assert
        if(nullableExpectedSyntax is { } expectedSyntax
            && !expectedSyntax.Equals(actualSyntax))
        {
            try
            {
                TokenAssertionVisitor.Instance.Verify(actualSyntax);
            } catch(XunitException ex)
            {
                Assert.Fail($"actual syntax was malformed:\n{ex}");
            }

            var left = expectedSyntax.ToAstString();
            var right = actualSyntax.ToAstString();
            TestHelpers.FailWithDiff(left, right);
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
        {{
            if(condition)
            {
                ((value))
            }
        }}
        """
        """",
        t => new(
        [
            new TextSyntax(t[0]),
            new CodeBlockSyntax(
                t[1],
                new([
                    new TextSyntax(t[2]),
                    new ValueBlockSyntax(new ValueHeadSyntax(t[3],new([new TextSyntax(t[4])]),t[5])),
                    new TextSyntax(t[6])
                    ]),
                t[7])
        ]));
    [Fact]
    public void Parser_Parses_complex_2() => TestParser(
        """"
        """
        foo
        {{
            if(condition)
            {
                var value2 = "World";
                ((value))<<
                    Hello, ((value2))!
                >>
            }
        }}
        """
        """",
        t => new(
        [
            new TextSyntax(t[0]),
            new CodeBlockSyntax(
                t[1],
                new([
                    new TextSyntax(t[2]),
                    new ValueBlockSyntax(
                        new (t[3],new([new TextSyntax(t[4])]),t[5]),
                        new(new(t[6],new([
                            new TextSyntax(t[7]),
                            new ValueBlockSyntax(new(t[8],new([new TextSyntax(t[9])]),t[10])),
                            new TextSyntax(t[11])
                            ]),
                            t[12]))),
                    new TextSyntax(t[13])
                    ]),
                t[14])
        ]));
    [Fact]
    public void Parser_Parses_complex_3() => TestParser(
        """"
        """
        foo
        {{
            if(condition)
            {
                var value2 = "World";
                \((value\))\<<
                    Hello, ((value2))!
                \>>
            }
        }}
        bar
        """
        """",
        t => new(
        [
            new TextSyntax(t[0]),
            new CodeBlockSyntax(
                t[1],
                new([
                    new TextSyntax(t[2]),
                    new TextSyntax(t[3]),
                    new TextSyntax(t[4]),
                    new TextSyntax(t[5]),
                    new ValueBlockSyntax(new(t[6], new([new TextSyntax(t[7])]), t[8])),
                    new TextSyntax(t[9]),
                    new TextSyntax(t[10])
                ]),
                t[11]),
            new TextSyntax(t[12]),
        ]));
    [Fact]
    public void Parser_Reports_UnexpectedToken1() => TestParser(
        """"
        """
        foo
        {{
            if(condition)
            {
                var value2 = "World";
                \((value))\<<
                    Hello, ((value2))!
                \>>
            }
        }}
        bar
        """
        """",
        diagnosticsFactory: t =>
        [
            Diagnostic.Create(
                Diagnostic.Ids.UnexpectedToken,
                DiagnosticSeverity.Error,
                t[4].TemplateString.Path,
                t[4].Spans.SourceSpan)
        ]);
    [Fact]
    public void Parser_Parses_complex_escaped1() => TestParser(
        """"
        """
        {{var foo = "foo";}}
        ((new BarTemplate(\)))
        ((Greeting))
        ((new LayoutTemplate(\)))<<
            <h1>((Greeting)), ((foo))!</h1>
        >>
        """
        """",
        t => new([
            new CodeBlockSyntax(t[0],new([new TextSyntax(t[1])]),t[2]),
            new ValueBlockSyntax(new(t[3],new([new TextSyntax(t[4]),new TextSyntax(t[5])]),t[6])),
            new ValueBlockSyntax(new(t[7],new([new TextSyntax(t[8])]),t[9])),
            new ValueBlockSyntax(
                new(t[10],new([new TextSyntax(t[11]),new TextSyntax(t[12])]),t[13]),
                new(new(t[14],new([
                    new TextSyntax(t[15]),
                    new ValueBlockSyntax(new(t[16],new([new TextSyntax(t[17])]),t[18])),
                    new TextSyntax(t[19]),
                    new ValueBlockSyntax(new(t[20],new([new TextSyntax(t[21])]),t[22])),
                    new TextSyntax(t[23])
                ]),t[24]))),
        ]));
    [Fact]
    public void Parser_Parses_complex_escaped2() => TestParser(
        """"
        """
        foo
        {{
            if(condition)
            {
                var value2 = "World";
                ((value))\<<
                    Hello, ((value2))!
                \>>
            }
        }}
        bar
        """
        """",
        t => new([
            new TextSyntax(t[0]),
            new CodeBlockSyntax(t[1],new([
                new TextSyntax(t[2]),
                new ValueBlockSyntax(new(t[3],new([new TextSyntax(t[4])]),t[5])),
                new TextSyntax(t[6]),
                new ValueBlockSyntax(new(t[7],new([new TextSyntax(t[8])]),t[9])),
                new TextSyntax(t[10]),
                new TextSyntax(t[11]),
            ]),t[12]),
            new TextSyntax(t[13])
        ]));
}
