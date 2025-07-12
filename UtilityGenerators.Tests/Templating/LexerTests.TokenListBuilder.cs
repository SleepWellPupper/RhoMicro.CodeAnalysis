// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Tests.Templating;

using System;
using System.Runtime.CompilerServices;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;

public partial class LexerTests
{
    private sealed class TokenListBuilder(TemplateString templateString, Int32 newlineLength)
    {
        private readonly List<Token> _tokens = [];

        public EquatableList<Token> Build(in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            var result = ctx.CollectionFactory.CreateList<Token>();

            foreach(var token in _tokens)
            {
                ctx.ThrowIfCancellationRequested();
                result.Add(token);
            }

            return result;
        }

        public TokenListBuilder OpenCodeBlock() => Token(TokenKind.OpenCodeBlock, 2);
        public TokenListBuilder CloseCodeBlock() => Token(TokenKind.CloseCodeBlock, 2);
        public TokenListBuilder OpenRenderBlock() => Token(TokenKind.OpenRenderBlock, 2);
        public TokenListBuilder CloseRenderBlock() => Token(TokenKind.CloseRenderBlock, 2);
        public TokenListBuilder OpenTemplateBlock() => Token(TokenKind.OpenTemplateBlock, 2);
        public TokenListBuilder CloseTemplateBlock() => Token(TokenKind.CloseTemplateBlock, 2);

        public TokenListBuilder NotNewline(Int32 length) => Token(TokenKind.NotNewline, length);

        public TokenListBuilder EscapedOpen(TokenKind kind) => Token(kind, 2).Token(TokenKind.EscapeColon, 1);
        public TokenListBuilder EscapedOpen(Int32 kind) => EscapedOpen((TokenKind)kind);
        public TokenListBuilder EscapedClose(TokenKind kind) => Token(TokenKind.EscapeColon, 1).Token(kind, 2);
        public TokenListBuilder EscapedClose(Int32 kind) => EscapedClose((TokenKind)kind);

        public TokenListBuilder EscapedOpenCodeBlock() => Token(TokenKind.OpenCodeBlock, 2).Token(TokenKind.EscapeColon, 1);
        public TokenListBuilder EscapedCloseCodeBlock() => Token(TokenKind.EscapeColon, 1).Token(TokenKind.CloseCodeBlock, 2);
        public TokenListBuilder EscapedOpenRenderBlock() => Token(TokenKind.OpenRenderBlock, 2).Token(TokenKind.EscapeColon, 1);
        public TokenListBuilder EscapedCloseRenderBlock() => Token(TokenKind.EscapeColon, 1).Token(TokenKind.CloseRenderBlock, 2);
        public TokenListBuilder EscapedOpenTemplateBlock() => Token(TokenKind.OpenTemplateBlock, 2).Token(TokenKind.EscapeColon, 1);
        public TokenListBuilder EscapedCloseTemplateBlock() => Token(TokenKind.EscapeColon, 1).Token(TokenKind.CloseTemplateBlock, 2);

        public TokenListBuilder Whitespaces(Int32 length) => Token(TokenKind.Whitespaces, length);
        public TokenListBuilder Newline(Int32 length) => Token(TokenKind.Newline, length);

        public TokenListBuilder Eof() => Token(TokenKind.Eof, 0);

        public TokenListBuilder Token(Int32 kind, Int32 length) => Token((TokenKind)kind, length);
        public TokenListBuilder Token(TokenKind kind, Int32 length)
        {
            var spans = _tokens is [..,
            {
                Spans:
                {
                    SourceSpan.End:
                    {
                        Line: var previousEndLine,
                        Character: var previousEndCharacter
                    },
                    TemplateSpan:
                    {
                        Index: var previousIndex,
                        Length: var previousLength
                    },
                    NewlineAwareTemplateSpan:
                    {
                        Index: var previousNewlineAwareIndex,
                        Length: var previousNewlineAwareLength
                    }
                }, Kind: var previousKind
            }]
                ? new TokenSpans(
                    TemplateSpan: new(
                        index: previousIndex + previousLength,
                        length: length),
                    NewlineAwareTemplateSpan: new(
                        index: previousNewlineAwareIndex+previousNewlineAwareLength,
                        length: kind is TokenKind.Newline
                            ? newlineLength
                            : length),
                    SourceSpan: new(
                        start: new(
                            line:
                                kind is TokenKind.Eof
                                ? previousEndLine
                                : previousKind is TokenKind.Newline
                                ? previousEndLine + 1
                                : previousEndLine,
                            character:
                                kind is TokenKind.Eof
                                ? previousEndCharacter + 1
                                : previousKind is TokenKind.Newline
                                ? templateString.Start.Character
                                : previousEndCharacter + 1),
                        end: new(
                            line:
                                kind is TokenKind.Eof
                                ? previousEndLine
                                : previousKind is TokenKind.Newline
                                ? previousEndLine + 1
                                : previousEndLine,
                            character:
                                kind is TokenKind.Eof
                                ? previousEndCharacter + 1
                                : previousKind is TokenKind.Newline
                                ? templateString.Start.Character + length - 1
                                : previousEndCharacter + length)))
                : new TokenSpans(
                    TemplateSpan: new(
                        index: 0,
                        length: length),
                    NewlineAwareTemplateSpan: new(
                        index: 0,
                        length: kind is TokenKind.Newline
                            ? newlineLength
                            : length),
                    SourceSpan: new(
                        start: new(
                            line: templateString.Start.Line,
                            character: templateString.Start.Character),
                        end: new(
                            line: templateString.Start.Line,
                            character: kind is TokenKind.Eof
                                ? templateString.Start.Character
                                : templateString.Start.Character + length - 1)));

            _tokens.Add(new Token(kind, templateString, spans));

            return this;
        }

        public override String ToString() => String.Join(" ", _tokens);
    }
}