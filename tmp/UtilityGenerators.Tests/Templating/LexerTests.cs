#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

using static RhoMicro.CodeAnalysis.Templating.TokenKind;

public class LexerTests
{
    private sealed class TokenListBuilder(TemplateString templateString)
    {
        private TokenKind? _currentKind;
        private Int32 _startLine = templateString.Start.Line;
        private Int32 _startCharacter = templateString.Start.Character;
        private Int32 _currentLine = templateString.Start.Line;
        private Int32 _currentCharacter = templateString.Start.Character;
        private Int32 _startIndex;
        private Int32 _currentIndex;

        private readonly List<Token> _tokens = [];

        public EquatableList<Token> Build(in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            Flush();

            var result = ctx.CollectionFactory.CreateList<Token>();

            foreach(var token in _tokens)
            {
                ctx.ThrowIfCancellationRequested();
                result.Add(token);
            }

            return result;
        }

        private void Flush()
        {
            if(_currentKind is not null)
            {
                var token = new Token(
                            _currentKind.Value,
                            templateString,
                            new(
                                new(_startIndex, _currentIndex - _startIndex),
                                new(
                                    new(_startLine, _startCharacter),
                                    new(_currentLine, _currentCharacter))));

                _tokens.Add(token);

                _currentCharacter++;
                _currentKind = null;
            }

            _startIndex = _currentIndex;
            _startLine = _currentLine;
            _startCharacter = _currentCharacter;
        }

        public TokenListBuilder Skip()
        {
            Flush();

            _currentKind = null;

            return this;
        }
        public TokenListBuilder Kind(Int32 kind) => Kind((TokenKind)kind);
        public TokenListBuilder Kind(TokenKind kind)
        {
            Flush();

            _currentKind = kind;

            return this;
        }
        public TokenListBuilder Length(Int32 length)
        {
            _currentIndex = _startIndex + length;
            _currentCharacter += length - 1;
            return this;
        }
        public TokenListBuilder Newline(Int32 count = 1)
        {
            _currentLine += count;
            _currentCharacter = templateString.Start.Character;
            return this;
        }
        public TokenListBuilder Character(Int32 character)
        {
            _currentCharacter = character;
            return this;
        }

        public override String ToString() => String.Join(" ", _tokens);
    }

    private static void TestLexer(String sourceText, Action<TokenListBuilder> buildTokens)
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
        var builder = new TokenListBuilder(templateString);
        buildTokens.Invoke(builder);
        EquatableList<Token> expected, actual;

        // Act
        using(var context = ModelCreationContext.CreateDefault(CancellationToken.None))
        {
            expected = builder.Build(in context);
            actual = Lexer.Scan(templateString, in context).Tokens;
        }

        // Assert
        if(!expected.Equals(actual))
        {
            var left = String.Join("\n", expected);
            var right = String.Join("\n", actual);
            TestHelpers.FailWithDiff(left, right, columnWidth: 32);
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
            .ParseText(sourceText)
            .GetRoot()
            .DescendantTokens(_ => true)
            .Single(t => t.RawKind is
                (Int32)SyntaxKind.StringLiteralToken or
                (Int32)SyntaxKind.MultiLineRawStringLiteralToken or
                (Int32)SyntaxKind.SingleLineRawStringLiteralToken);

        var templateString = TemplateString.Create(token, CancellationToken.None);
        Int32 actual;

        // Act
        using(var context = ModelCreationContext.CreateDefault(CancellationToken.None))
            actual = Lexer.Scan(templateString, in context).RequiredQuotes;

        // Assert
        Assert.Equal(expected, actual);
    }

    public static TheoryData<String, Int32, String, Int32> Matrix =>
            new()
            {
                { "foo", 0, "foo", 0 },
                { "bar", 1, "bar", 1 }
            };
    
    [Theory]
    [MemberData(nameof(Matrix))]
    //[InlineData("(:", (Int32)OpenRenderBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData("(:", (Int32)OpenRenderBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData("(:", (Int32)OpenRenderBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData("(:", (Int32)OpenRenderBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData("(:", (Int32)OpenRenderBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData("(:", (Int32)OpenRenderBlock, ":>", (Int32)CloseTemplateBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData(":)", (Int32)CloseRenderBlock, ":>", (Int32)CloseTemplateBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData("{:", (Int32)OpenCodeBlock, ":>", (Int32)CloseTemplateBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData(":}", (Int32)CloseCodeBlock, ":>", (Int32)CloseTemplateBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData("<:", (Int32)OpenTemplateBlock, ":>", (Int32)CloseTemplateBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, "(:", (Int32)OpenRenderBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, ":)", (Int32)CloseRenderBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, "{:", (Int32)OpenCodeBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, ":}", (Int32)CloseCodeBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, "<:", (Int32)OpenTemplateBlock)]
    //[InlineData(":>", (Int32)CloseTemplateBlock, ":>", (Int32)CloseTemplateBlock)]
    public void Lexer_omits_newline_between_block_terminators1(String terminator, Int32 kind1, String nonTerminator, Int32 kind2) => TestLexer(
        $$""""
        """
        foo{{terminator}}
        {{nonTerminator}}bar
        """
        """", b => b
           .Kind(Text).Length(3)
           .Kind(kind1).Length(2)
           .Skip().Length(1).Newline()
           .Kind(kind2).Length(2)
           .Kind(Text).Length(3)
           .Kind(Eof));
    [Theory]
    [InlineData("(:", (Int32)OpenRenderBlock)]
    [InlineData(":)", (Int32)CloseRenderBlock)]
    [InlineData("{:", (Int32)OpenCodeBlock)]
    [InlineData(":}", (Int32)CloseCodeBlock)]
    [InlineData("<:", (Int32)OpenTemplateBlock)]
    [InlineData(":>", (Int32)CloseTemplateBlock)]
    public void LexerDoesNotOmitNewlineAfterBlockTerminatorFollowedByText(String terminator, Int32 kind) => TestLexer(
        $$""""
        """
        foo{{terminator}}
        foo
        """
        """", b => b
           .Kind(Text).Length(3)
           .Kind(kind).Length(2)
           .Kind(Text).Length(4).Newline().Character(2)
           .Kind(Eof));

    [Theory]
    [InlineData("(::")]
    [InlineData("{::")]
    [InlineData("<::")]
    public void LexerScansEscapedOpenAsText(String escaped) => TestLexer(
        $$""""
        """
        foo{{escaped}}bar
        """
        """", b => b
           .Kind(Text).Length(5)
           .Skip().Length(1)
           .Kind(Text).Length(3)
           .Kind(Eof)
        );

    [Theory]
    [InlineData("::)")]
    [InlineData("::}")]
    [InlineData("::>")]
    public void LexerScansEscapedCloseAsText(String escaped) => TestLexer(
        $$""""
        """
        foo{{escaped}}bar
        """
        """", b => b
           .Kind(Text).Length(3)
           .Skip().Length(1)
           .Kind(Text).Length(5)
           .Kind(Eof)
        );
    [Theory]
    [InlineData("(::)", (Int32)OpenRenderBlock, (Int32)CloseRenderBlock)]
    [InlineData("{::}", (Int32)OpenCodeBlock, (Int32)CloseCodeBlock)]
    [InlineData("<::>", (Int32)OpenTemplateBlock, (Int32)CloseTemplateBlock)]
    public void LexerIncludesBlock(String block, Int32 leftKind, Int32 rightKind) => TestLexer(
        $$""""
        """
        foo{{block}}bar
        """
        """", b => b
           .Kind(Text).Length(3)
           .Kind(leftKind).Length(2)
           .Kind(rightKind).Length(2)
           .Kind(Text).Length(3)
           .Kind(Eof)
        );
}
