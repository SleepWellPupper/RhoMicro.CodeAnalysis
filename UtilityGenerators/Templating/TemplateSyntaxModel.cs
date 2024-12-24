namespace RhoMicro.CodeAnalysis.Templating;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record TemplateSyntaxModel(SourceSpanModel Source, EquatableList<TemplateChildSyntaxModel> Children) : TemplateSyntaxBaseModel(Source, SyntaxKind.Template)
{
    public static TemplateSyntaxModel Parse(String sourceText, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var children = ctx.CollectionFactory.CreateList<TemplateChildSyntaxModel>();

        if(sourceText.Length == 0)
            return new TemplateSyntaxModel(sourceText, children);

        var position = 0;

        while(!isAtEnd())
        {
            ctx.ThrowIfCancellationRequested();

            if(peek() == '§')
            {
                if(match(['§', '('], in ctx))
                {
                    children.Add(parseValue(in ctx));
                } else if(match(['§', '{'], in ctx))
                {
                    children.Add(parseCode(in ctx));
                } else
                {
                    // Handle diagnostic: Invalid sequence starting with '§'
                    advance(); // Skip the invalid marker
                }
            } else
            {
                parseTemplateTextChildren(children, in ctx);
            }
        }

        return new TemplateSyntaxModel(sourceText, children);

        ValueSyntaxModel parseValue(in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            var start = position - 2; // Include "§("
            var openParens = 1;

            while(!isAtEnd() && openParens > 0)
            {
                ctx.ThrowIfCancellationRequested();

                if(peek() == ')')
                    openParens--;
                else if(peek() == '(')
                    openParens++;

                advance();

                    if(openParens == 0)
                        break;
            }

            var length = position - start;

            return new ValueSyntaxModel(new(start, length, sourceText));
        }

        CodeSyntaxModel parseCode(in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            var start = position - 2; // Include "§{"
            var children = ctx.CollectionFactory.CreateList<TextOrValueSyntaxModel>();
            var requiredClosingBraceCount = 1;

            while(!isAtEnd() && requiredClosingBraceCount > 0)
            {
                ctx.ThrowIfCancellationRequested();

                if(peek() == '§')
                {
                    if(match(['§', '('], in ctx))
                    {
                        children.Add(parseValue(in ctx));
                    } else
                    {
                        // Handle diagnostic: Invalid sequence inside code hole
                        advance(); // Skip invalid marker
                    }
                } else
                {
                    parseCodeTextChildren(children, ref requiredClosingBraceCount, in ctx);
                }
            }

            if(isAtEnd())
            {
                // Handle diagnostic: Missing closing brace for code hole
            }

            var length = position - start;

            return new CodeSyntaxModel(new(start, length, sourceText), children);
        }

        void parseTemplateTextChildren(EquatableList<TemplateChildSyntaxModel> children, in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            var start = position;

            while(!isAtEnd() && peek() != '§')
            {
                ctx.ThrowIfCancellationRequested();

                if(peek() is '\\')
                {
                    // consume esc
                    advance();

                    // esc was last char in source => diagnostic
                    if(isAtEnd())
                        break;

                    // followed by escapable?
                    if(peek() is '\\' or '§')
                    {
                        // add text up to esc
                        advance(-1);
                        add();
                        // advance to esc
                        advance();
                        // reset start for next text
                        start = position;
                        // advance past escaped
                        advance();

                        // continue so when we add diagnostics later, they won't get emitted
                        continue;
                    }

                    // not followed by escapable => diagnostic
                    // esc is included in text
                } else
                {
                    // consume non-escape char
                    advance();
                }
            }

            // remaining text
            add();

            void add()
            {
                if(position != start)
                    children.Add(new TextSyntaxModel(new(start, length: position - start, sourceText)));
            }
        }

        void parseCodeTextChildren(EquatableList<TextOrValueSyntaxModel> children, ref Int32 requiredClosingBraceCount, in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            var start = position;

            while(!isAtEnd() && peek() != '§')
            {
                ctx.ThrowIfCancellationRequested();

                if(peek() == '{')
                    requiredClosingBraceCount++;

                if(peek() == '}')
                {
                    requiredClosingBraceCount--;
                    if(requiredClosingBraceCount == 0)
                    {
                        // consume up to code closing brace
                        add();
                        // consume closing brace
                        advance();
                        // reset position so we don't add the closing brace
                        start = position;
                        // exit
                        break;
                    }
                }

                if(peek() is '\\')
                {
                    // consume esc
                    advance();

                    // esc was last char in source => diagnostic
                    if(isAtEnd())
                        break;

                    // followed by escapable?
                    if(peek() is '\\' or '§')
                    {
                        // add text up to esc
                        advance(-1);
                        add();
                        // advance to esc
                        advance();
                        // reset start for next text
                        start = position;
                        // advance past escaped
                        advance();

                        // continue so when we add diagnostics later, they won't get emitted
                        continue;
                    }

                    // not followed by escapable => diagnostic
                    // esc is included in text
                } else
                {
                    // consume non-escape char
                    advance();
                }
            }

            // remaining text
            add();

            void add()
            {
                if(position != start)
                    children.Add(new TextSyntaxModel(new(start, length: position - start, sourceText)));
            }
        }

        Boolean match(ReadOnlySpan<Char> expected, in ModelCreationContext ctx)
        {
            ctx.ThrowIfCancellationRequested();

            if(position + expected.Length > sourceText.Length)
                return false;

            if(sourceText.AsSpan(position, expected.Length).SequenceEqual(expected))
            {
                advance(expected.Length);
                return true;
            }

            return false;
        }

        Char peek() => position < sourceText.Length ? sourceText[position] : '\0';

        // Char lookAhead(Int32 offset) => position + offset < sourceText.Length ? sourceText[position + offset] : '\0';

        void advance(Int32 count = 1) => position += count;

        Boolean isAtEnd() => position >= sourceText.Length;
    }
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
    public override String ToDebugString() => $"[{SyntaxKind} {String.Concat(Children.Select(c => $"{c.ToDebugString()}"))} ]";
#endif
    public override String ToString() => base.ToString();
}
