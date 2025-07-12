// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
using System;
using System.Threading;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile(Hint = "RhoMicro_CodeAnalysis_Library_Text_Templating_TemplateRenderer_Render.g.cs")]
#endif
internal partial struct TemplateRenderer
{
    public static String Render<T>(in T template, CancellationToken cancellationToken = default)
        where T : ITemplate
        => Render(template, EmptyTemplate.Instance, cancellationToken);
    public static String Render<T, TBody>(in T template, in TBody body, CancellationToken cancellationToken = default)
        where T : ITemplate
        where TBody : ITemplate
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ctx = new TemplateRenderer(stackalloc Char[2048], stackalloc Char[64], cancellationToken);
        try
        {
            template.Render(ref ctx, body, cancellationToken);
        } finally
        {
            ctx.Dispose();
        }

        var result = ctx.ToString();

        return result;
    }

    public void Render(params ReadOnlySpan<Char> value)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        // If there is no indentation requested, we omit any and all checks for
        // newlines and splitting at newlines.
        if(Indentation is [])
        {
            _buffer.Add(value);
            return;
        }

        // If the value is empty, we do not unnecessarily prepend indentation,
        // as no newline could be present.
        if(value is [])
            return;

        // Otherwise, we split at each newline and append the requested
        // indentation if the next line is not empty. Since the value will be
        // preceded by the buffer, we determine whether or not it is preceded by
        // a newline by looking at the last two characters of the buffer. In
        // order to split the value, we must track the start and length (via i)
        // of the current span examined. The currently examined span is a line
        // beginning with a non-newline and optionally followed by any number of
        // empty newlines. We treat an empty buffer as if it contained a single
        // newline. 
        var precededByNewline = _buffer.Span is [] or [.., '\n'] or [.., '\r', '\n'];
        var start = 0;
        var i = 0;
        for(; i < value.Length; i++)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var newlineCharacterCount =
                value[i] == '\n'
                ? 1
                : i + 1 < value.Length && value[i] == '\r' && value[i + 1] == '\n'
                ? 2
                : 0;
            var isNewLine = newlineCharacterCount > 0;

            // If we are encountering a newline, we set the flag for detection
            // on the next iteration. We will append indentation upon
            // encountering a non-newline in the next iteration.
            if(isNewLine)
            {
                i += newlineCharacterCount - 1;
                precededByNewline = true;
                continue;
            }

            // We have encountered a non-newline character and are preceded by a
            // newline, so we first append the examined span.
            if(precededByNewline)
            {
                var length = i - start;
                var examinedSpan = value.Slice(start, length);
                // We first add the examined span.
                _buffer.Add(examinedSpan);
                // And then the indentation (to be followed by the next examined
                // span upon further iteration.
                _buffer.Add(Indentation);
                // We prepare the next span b resetting the starting index.
                start = i;
            }

            // The next character will not be preceded by a newline, so we unset
            // the flag.
            precededByNewline = false;
        }

        // We add the remaining characters.
        if(start != i)
        {
            var length = i - start;
            var examinedSpan = value.Slice(start, length);
            _buffer.Add(examinedSpan);
        }
    }

    public void Render(ReadOnlySpan<Char> customIndentation, ReadOnlySpan<Char> value)
    {
        Indent(customIndentation);
        Render(value);
        Detent(customIndentation.Length);
    }
    public void Render(String value) => Render(value.AsSpan());
    public void Render(ReadOnlySpan<Char> customIndentation, String value) => Render(customIndentation, value.AsSpan());

    public void Render<T>(in T value)
        where T : ITemplate
        => Render(in value, EmptyTemplate.Instance);
    public void Render<T, TBody>(in T value, in TBody body)
        where T : ITemplate
        where TBody : ITemplate 
        => value.Render(ref this, body, _cancellationToken);
    public void Render<T>(in T value, RenderFragment fragment)
        where T : ITemplate
        => Render(in value, new RenderFragmentAdapter(fragment));
    public void Render<T>(ReadOnlySpan<Char> customIndentation, in T value)
        where T : ITemplate
        => Render(customIndentation, in value, EmptyTemplate.Instance);
    public void Render<T>(ReadOnlySpan<Char> customIndentation, in T value, RenderFragment fragment)
        where T : ITemplate
        => Render(customIndentation, in value, new RenderFragmentAdapter(fragment));
    public void Render<T, TBody>(ReadOnlySpan<Char> customIndentation, in T value, in TBody body)
        where T : ITemplate
        where TBody : ITemplate
    {
        Indent(customIndentation);
        Render(value, in body);
        Detent(customIndentation.Length);
    }
}
