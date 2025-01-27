#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

using Xunit.Runner.Common;

using static RhoMicro.CodeAnalysis.Templating.TokenKind;

public partial class LexerTests(ITestOutputHelper testOutput)
{
    private void TestLexer(String sourceText, Action<TokenListBuilder> buildTokens)
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
        var builder = new TokenListBuilder(templateString);
        buildTokens.Invoke(builder);
        EquatableList<Token> expected, actual;

        // Act
        using(var context = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
        {
            expected = builder.Build(in context);
            actual = Lexer.Scan(templateString, in context).Tokens;
        }

        // Assert
        if(!expected.Equals(actual))
        {
            var left = String.Join("\n", expected);
            var right = String.Join("\n", actual);
            TestHelpers.FailWithDiff(left, right, columnWidth: 48);
        } else
        {
            testOutput.WriteLine($"template string text:\n{templateString.Text}\n\ntokens:\n{String.Join("\n", expected)}");
        }
    }

    [Theory]
    [InlineData(
        """
        "foo"
        """, 3)]
    [InlineData(
        """"
        """foo"""
        """", 3)]
    [InlineData(
        """""
        """"foo""""
        """"", 3)]
    [InlineData(
        """"""
        """""foo"""""
        """""", 3)]
    [InlineData(
        """""
        """" """foo""" """"
        """"", 4)]
    [InlineData(
        """"""
        """"" """"foo"""" """"
        """""", 5)]
    [InlineData(
        """"
        """
        foo
        """
        """", 3)]
    [InlineData(
        """""
        """"
        foo
        """"
        """"", 3)]
    [InlineData(
        """"""
        """""
        foo
        """""
        """""", 3)]
    [InlineData(
        """""
        """"
        """foo"""
        """"
        """"", 4)]
    [InlineData(
        """"""
        """""
        """"foo""""
        """""
        """""", 5)]
    public void LexerRecognizesRequiredQuotes(String sourceText, Int32 expected)
    {
        // Arrange
        var token = CSharpSyntaxTree
            .ParseText(sourceText, cancellationToken: TestContext.Current.CancellationToken)
            .GetRoot(TestContext.Current.CancellationToken)
            .DescendantTokens(_ => true)
            .Single(t => t.RawKind is
                (Int32)SyntaxKind.StringLiteralToken or
                (Int32)SyntaxKind.MultiLineRawStringLiteralToken or
                (Int32)SyntaxKind.SingleLineRawStringLiteralToken);

        var templateString = TemplateString.Create(token, TestContext.Current.CancellationToken);
        Int32 actual;

        // Act
        using(var context = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
            actual = Lexer.Scan(templateString, in context).RequiredQuotes;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock, "")]
    [InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock, "")]
    [InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock, "")]
    [InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock, "")]
    [InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock, "")]
    [InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock, "")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock, "")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock, "")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock, "")]

    [InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock, " ")]
    [InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock, " ")]
    [InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock, " ")]
    [InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock, " ")]
    [InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock, " ")]
    [InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock, " ")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock, " ")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock, " ")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock, " ")]

    [InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock, "\t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock, "\t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock, "\t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock, "\t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock, "\t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock, "\t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock, "\t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock, "\t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock, "\t")]

    [InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock, "\t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock, "\t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock, "\t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock, "\t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock, "\t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock, "\t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock, "\t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock, "\t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock, "\t \t")]

    [InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock, "   \t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock, "   \t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock, "   \t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock, "   \t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock, "   \t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock, "   \t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock, "   \t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock, "   \t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock, "   \t \t")]
    public void LexerScansTriviaBetweenBlocks(String terminator1, Int32 kind1, String terminator2, Int32 kind2, String whitespace) => TestLexer(
        $""""
        """
        foo{terminator1}
        {whitespace}{terminator2}bar
        """
        """", b => b
           .Text(3)
           .Kind(kind1).Length(2)
           .Trivia(whitespace.Length)
           .Kind(kind2).Length(2)
           .Text(3)
           .Eof());
    [Theory]
    [InlineData("(:", (Int32)OpenRenderBlock, "")]
    [InlineData(":)", (Int32)CloseRenderBlock, "")]
    [InlineData("{:", (Int32)OpenCodeBlock, "")]
    [InlineData(":}", (Int32)CloseCodeBlock, "")]
    [InlineData("<:", (Int32)OpenTemplateBlock, "")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "")]

    [InlineData("(:", (Int32)OpenRenderBlock, " ")]
    [InlineData(":)", (Int32)CloseRenderBlock, " ")]
    [InlineData("{:", (Int32)OpenCodeBlock, " ")]
    [InlineData(":}", (Int32)CloseCodeBlock, " ")]
    [InlineData("<:", (Int32)OpenTemplateBlock, " ")]
    [InlineData(":>", (Int32)CloseTemplateBlock, " ")]

    [InlineData("(:", (Int32)OpenRenderBlock, "\t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "\t")]
    [InlineData("{:", (Int32)OpenCodeBlock, "\t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "\t")]
    [InlineData("<:", (Int32)OpenTemplateBlock, "\t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "\t")]

    [InlineData("(:", (Int32)OpenRenderBlock, "\t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "\t \t")]
    [InlineData("{:", (Int32)OpenCodeBlock, "\t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "\t \t")]
    [InlineData("<:", (Int32)OpenTemplateBlock, "\t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "\t \t")]

    [InlineData("(:", (Int32)OpenRenderBlock, "   \t \t")]
    [InlineData(":)", (Int32)CloseRenderBlock, "   \t \t")]
    [InlineData("{:", (Int32)OpenCodeBlock, "   \t \t")]
    [InlineData(":}", (Int32)CloseCodeBlock, "   \t \t")]
    [InlineData("<:", (Int32)OpenTemplateBlock, "   \t \t")]
    [InlineData(":>", (Int32)CloseTemplateBlock, "   \t \t")]
    public void LexerDoesNotScanTriviaBetweenBlockAndText(String terminator, Int32 kind, String whitespace) => TestLexer(
        $""""
        """
        foo{terminator}
        {whitespace}foo
        """
        """", b => b
           .Text(3)
           .Kind(kind).Length(2)
           .Text(4 + whitespace.Length).Newline().Character(whitespace.Length + 2)
           .Eof());

    [Theory]
    [InlineData("(::", (Int32)OpenRenderBlock)]
    [InlineData("{::", (Int32)OpenCodeBlock)]
    [InlineData("<::", (Int32)OpenTemplateBlock)]
    public void LexerScansEmbeddedEscapedOpen(String escaped, Int32 kind) => TestLexer(
        $""""
        """
        foo{escaped}bar
        """
        """", b => b
           .Text(3)
           .EscapedOpen(kind)
           .Text(3)
           .Eof()
        );
    [Theory]
    [InlineData("(::", (Int32)OpenRenderBlock)]
    [InlineData("{::", (Int32)OpenCodeBlock)]
    [InlineData("<::", (Int32)OpenTemplateBlock)]
    public void LexerScansSimpleEscapedOpen(String escaped, Int32 kind) => TestLexer(
        $""""
        """
        {escaped}
        """
        """", b => b
           .EscapedOpen(kind)
           .Eof()
        );

    [Theory]
    [InlineData("::)", (Int32)CloseRenderBlock)]
    [InlineData("::}", (Int32)CloseCodeBlock)]
    [InlineData("::>", (Int32)CloseTemplateBlock)]
    public void LexerScansEmbeddedEscapedClose(String escaped, Int32 kind) => TestLexer(
        $""""
        """
        foo{escaped}bar
        """
        """", b => b
           .Text(3)
           .EscapedClose(kind)
           .Text(3)
           .Eof()
        );
    [Theory]
    [InlineData("::)", (Int32)CloseRenderBlock)]
    [InlineData("::}", (Int32)CloseCodeBlock)]
    [InlineData("::>", (Int32)CloseTemplateBlock)]
    public void LexerScansSimpleEscapedClose(String escaped, Int32 kind) => TestLexer(
        $""""
        """
        {escaped}
        """
        """", b => b
           .EscapedClose(kind)
           .Eof()
        );
    [Theory]
    [InlineData("(::)", (Int32)OpenRenderBlock, (Int32)CloseRenderBlock)]
    [InlineData("{::}", (Int32)OpenCodeBlock, (Int32)CloseCodeBlock)]
    [InlineData("<::>", (Int32)OpenTemplateBlock, (Int32)CloseTemplateBlock)]
    public void LexerScansEmbeddedEmptyBlock(String block, Int32 leftKind, Int32 rightKind) => TestLexer(
        $""""
        """
        foo{block}bar
        """
        """", b => b
           .Text(3)
           .Kind(leftKind).Length(2)
           .Kind(rightKind).Length(2)
           .Text(3)
           .Eof()
        );
    [Theory]
    [InlineData("(::)", (Int32)OpenRenderBlock, (Int32)CloseRenderBlock)]
    [InlineData("{::}", (Int32)OpenCodeBlock, (Int32)CloseCodeBlock)]
    [InlineData("<::>", (Int32)OpenTemplateBlock, (Int32)CloseTemplateBlock)]
    public void LexerScansSimpleEmptyBlock(String block, Int32 leftKind, Int32 rightKind) => TestLexer(
        $""""
        """
        {block}
        """
        """", b => b
           .Kind(leftKind).Length(2)
           .Kind(rightKind).Length(2)
           .Eof()
        );
    [Theory]
    [InlineData("(:", (Int32)OpenRenderBlock, ":)", (Int32)CloseRenderBlock)]
    [InlineData("{:", (Int32)OpenCodeBlock, ":}", (Int32)CloseCodeBlock)]
    [InlineData("<:", (Int32)OpenTemplateBlock, ":>", (Int32)CloseTemplateBlock)]
    public void LexerScansSimpleBlock(
        String leftTerminator,
        Int32 leftKind,
        String rightTerminator,
        Int32 rightKind) => TestLexer(
        $""""
        """
        {leftTerminator}foo{rightTerminator}
        """
        """", b => b
           .Kind(leftKind).Length(2)
           .Text(3)
           .Kind(rightKind).Length(2)
           .Eof()
        );
    [Theory]
    [InlineData("(:", (Int32)OpenRenderBlock)]
    [InlineData(":)", (Int32)CloseRenderBlock)]
    [InlineData("{:", (Int32)OpenCodeBlock)]
    [InlineData(":}", (Int32)CloseCodeBlock)]
    [InlineData("<:", (Int32)OpenTemplateBlock)]
    [InlineData(":>", (Int32)CloseTemplateBlock)]
    [InlineData("foo", (Int32)Text)]
    [InlineData(" ", (Int32)Text)]
    public void LexerScansSingleToken(String lexeme, Int32 kind) => TestLexer(
        $""""
        """{lexeme}"""
        """", b => b
        .Kind(kind).Length(lexeme.Length)
        .Eof()
        );
    [Fact]
    public void LexerScansEmptyAsEof() =>
        // empty raw string literals are illegal
        TestLexer("\"\"", b => b.Eof());
    [Fact]
    public void LexerScansComplexText1() => TestLexer(
        """"
        """
        foo(::)baz
        {:bar:}
        (:)::>>
        """
        """", b => b
        .Text(3)                            // foo
        .OpenRenderBlock()                  // (:
        .CloseRenderBlock()                 // :)
        .Text(4).Flush().Newline()          // baz\n
        .OpenCodeBlock()                    // {:
        .Text(3)                            // bar
        .CloseCodeBlock()                   // :}
        .Trivia(0)                          // \n
        .OpenRenderBlock()                  // (:
        .Text(1)                            // )
        .EscapedClose(CloseTemplateBlock)   // ::>
        .Text(1)                            // >
        .Eof()
        );
}