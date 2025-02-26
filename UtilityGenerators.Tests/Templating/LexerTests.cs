#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

using static RhoMicro.CodeAnalysis.Templating.TokenKind;

public partial class LexerTests(ITestOutputHelper testOutput)
{
    private void TestLexer(String sourceText, Action<TokenListBuilder> buildTokens, Int32 newlineLength = 1)
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
        var builder = new TokenListBuilder(templateString, newlineLength);
        buildTokens.Invoke(builder);
        EquatableList<Token> expected, actual;

        // Act
        using(var context = ModelCreationContext.CreateDefault(TestContext.Current.CancellationToken))
        {
            expected = builder.Build(in context);
            actual = Lexer.Scan(templateString, newlineLength, in context).Tokens;
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
            actual = Lexer.Scan(templateString, newlineLength: 1, in context).RequiredQuotes;

        // Assert
        Assert.Equal(expected, actual);
    }

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
           .NotNewline(3)
           .EscapedOpen(kind)
           .NotNewline(3)
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
           .NotNewline(3)
           .EscapedClose(kind)
           .NotNewline(3)
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
           .NotNewline(3)
           .Token(leftKind, 2)
           .Token(rightKind, 2)
           .NotNewline(3)
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
           .Token(leftKind, 2)
           .Token(rightKind, 2)
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
           .Token(leftKind, 2).NotNewline(3).Token(rightKind, 2)
           .Eof()
        );
    [Theory]
    [InlineData("(:", (Int32)OpenRenderBlock)]
    [InlineData(":)", (Int32)CloseRenderBlock)]
    [InlineData("{:", (Int32)OpenCodeBlock)]
    [InlineData(":}", (Int32)CloseCodeBlock)]
    [InlineData("<:", (Int32)OpenTemplateBlock)]
    [InlineData(":>", (Int32)CloseTemplateBlock)]
    [InlineData("foo", (Int32)NotNewline)]
    [InlineData(" ", (Int32)Whitespaces)]
    [InlineData("\t", (Int32)Whitespaces)]
    [InlineData("\t ", (Int32)Whitespaces)]
    [InlineData(" \t", (Int32)Whitespaces)]
    [InlineData(" \t ", (Int32)Whitespaces)]
    [InlineData("\r", (Int32)Newline)]
    [InlineData("\r\n", (Int32)Newline)]
    [InlineData("\n", (Int32)Newline)]
    public void LexerScansSingleToken(String lexeme, Int32 kind) => TestLexer(
        $""""
        """
        {lexeme}
        """
        """", b => b
        .Token(kind, lexeme.Length)
        .Eof()
        );
    [Fact]
    public void LexerScansEmptyAsEof() =>
        // empty raw string literals are illegal
        TestLexer("\"\"", b => b.Eof());

    [Theory]
    [InlineData(" ", "{:", (Int32)OpenCodeBlock)]
    [InlineData("\t", "{:", (Int32)OpenCodeBlock)]
    [InlineData("\t ", "{:", (Int32)OpenCodeBlock)]
    [InlineData(" \t", "{:", (Int32)OpenCodeBlock)]

    [InlineData(" ", "<:", (Int32)OpenTemplateBlock)]
    [InlineData("\t", "<:", (Int32)OpenTemplateBlock)]
    [InlineData("\t ", "<:", (Int32)OpenTemplateBlock)]
    [InlineData(" \t", "<:", (Int32)OpenTemplateBlock)]
    public void LexerScansLeadingTrivia(String trivia, String open, Int32 kind) => TestLexer(
        $""""
        """
        foo
        {trivia}{open}bar
        """
        """", b => b
        .NotNewline(3).Newline(
               """
               

               """.Length)
        .Whitespaces(trivia.Length).Token(kind, open.Length).NotNewline(3)
        .Eof());

    [Theory]
    [InlineData("\n", ":}", (Int32)CloseCodeBlock)]
    [InlineData("\r\n", ":}", (Int32)CloseCodeBlock)]

    [InlineData("\n", ":>", (Int32)CloseTemplateBlock)]
    [InlineData("\r\n", ":>", (Int32)CloseTemplateBlock)]
    public void LexerScansTrailingTrivia(String trivia, String close, Int32 kind) => TestLexer(
        $""""
        """
        bar{close}{trivia}bar
        """
        """", b => b
        .NotNewline(3).Token(kind, close.Length).Newline(trivia.Length)
        .NotNewline(3)
        .Eof());
    [Theory]
    [InlineData("!")]
    public void LexerScansSingleTokenAsNotNewline(String token) => TestLexer(
        $""""
        """
        {token}
        """
        """", b => b
        .NotNewline(token.Length)
        .Eof());
    [Fact]
    public void LexerScansNewlineCorrectly() => TestLexer(
        """"
        """
        foo
        {:
            if(condition)
        """
        """", b => b
        .NotNewline(3)
        .Newline(2)
        .OpenCodeBlock()
        .Newline(2)
        .NotNewline(17)
        .Eof());
}