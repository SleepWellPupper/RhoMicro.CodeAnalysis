#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

public partial class LexerTests
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

            FlushCore();

            var result = ctx.CollectionFactory.CreateList<Token>();

            foreach(var token in _tokens)
            {
                ctx.ThrowIfCancellationRequested();
                result.Add(token);
            }

            return result;
        }

        private void FlushCore()
        {
            if(_currentKind is { } currentKind)
            {
                if(currentKind is TokenKind.Trivia && _whitespaceCharacters > 0)
                {
                    _ = Newline();
                    _currentCharacter = _whitespaceCharacters - 1;
                    _currentIndex += _whitespaceCharacters;
                }

                var token = new Token(
                            currentKind,
                            templateString,
                            new(
                                new(_startIndex, _currentIndex - _startIndex),
                                new(
                                    new(_startLine, _startCharacter),
                                    new(_currentLine, _currentCharacter))));

                _tokens.Add(token);

                _currentCharacter++;
                _currentKind = null;

                if(currentKind is TokenKind.Trivia && _whitespaceCharacters == 0)
                    _ = Newline();
            }

            _startIndex = _currentIndex;
            _startLine = _currentLine;
            _startCharacter = _currentCharacter;
            _whitespaceCharacters = 0;
        }

        public TokenListBuilder OpenCodeBlock() => Kind(TokenKind.OpenCodeBlock).Length(2);
        public TokenListBuilder CloseCodeBlock() => Kind(TokenKind.CloseCodeBlock).Length(2);
        public TokenListBuilder OpenRenderBlock() => Kind(TokenKind.OpenRenderBlock).Length(2);
        public TokenListBuilder CloseRenderBlock() => Kind(TokenKind.CloseRenderBlock).Length(2);
        public TokenListBuilder OpenTemplateBlock() => Kind(TokenKind.OpenTemplateBlock).Length(2);
        public TokenListBuilder CloseTemplateBlock() => Kind(TokenKind.CloseTemplateBlock).Length(2);

        public TokenListBuilder Text(Int32 length = 0) => Kind(TokenKind.Text).Length(length);

        public TokenListBuilder Flush()
        {
            FlushCore();

            return this;
        }

        public TokenListBuilder EscapedOpen(TokenKind kind) => Kind(kind).Length(2).Kind(TokenKind.EscapeColon).Length(1);
        public TokenListBuilder EscapedOpen(Int32 kind) => EscapedOpen((TokenKind)kind);
        public TokenListBuilder EscapedClose(TokenKind kind) => Kind(TokenKind.EscapeColon).Length(1).Kind(kind).Length(2);
        public TokenListBuilder EscapedClose(Int32 kind) => EscapedClose((TokenKind)kind);

        public TokenListBuilder EscapedOpenCodeBlock() => Kind(TokenKind.OpenCodeBlock).Length(2).Kind(TokenKind.EscapeColon).Length(1);
        public TokenListBuilder EscapedCloseCodeBlock() => Kind(TokenKind.EscapeColon).Length(1).Kind(TokenKind.CloseCodeBlock).Length(2);
        public TokenListBuilder EscapedOpenRenderBlock() => Kind(TokenKind.OpenRenderBlock).Length(2).Kind(TokenKind.EscapeColon).Length(1);
        public TokenListBuilder EscapedCloseRenderBlock() => Kind(TokenKind.EscapeColon).Length(1).Kind(TokenKind.CloseRenderBlock).Length(2);
        public TokenListBuilder EscapedOpenTemplateBlock() => Kind(TokenKind.OpenTemplateBlock).Length(2).Kind(TokenKind.EscapeColon).Length(1);
        public TokenListBuilder EscapedCloseTemplateBlock() => Kind(TokenKind.EscapeColon).Length(1).Kind(TokenKind.CloseTemplateBlock).Length(2);

        private Int32 _whitespaceCharacters;

        public TokenListBuilder Trivia(Int32 whitespaceLength)
        {
            _ = Kind(TokenKind.Trivia).Length(1);
            _whitespaceCharacters = whitespaceLength;
            return this;
        }

        public TokenListBuilder Eof() => Kind(TokenKind.Eof);
        public TokenListBuilder Kind(Int32 kind) => Kind((TokenKind)kind);
        public TokenListBuilder Kind(TokenKind kind)
        {
            FlushCore();

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
        public TokenListBuilder Character(Func<Int32, Int32> characterFactory) =>
            Character(characterFactory.Invoke(_currentCharacter));
        public TokenListBuilder Character(Int32 character)
        {
            _currentCharacter = character;
            return this;
        }

        public override String ToString() => String.Join(" ", _tokens);
    }
}