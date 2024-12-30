namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides helpers for writing data to a buffer of chars.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class TemplateRuntimeHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(ReadOnlySpan<Char> value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // If there is no indentation requested, we omit any and all checks for
        // newlines and splitting at newlines.
        if(indentation.Length == 0)
        {
            buffer.Add(value);
            return;
        }

        // If the value is empty, we do not unnecessarily prepend indentation,
        // as no newline could be present.
        if(value.Length == 0)
            return;

        // We attempt to locate the first newline.
        var newLineIndex = value.IndexOf('\n');

        // If indentation was requested but no newline was found, we simply
        // prepend indentation.
        if(newLineIndex == -1)
        {
            buffer.Add(indentation);
            buffer.Add(value);
            return;
        }

        // Otherwise, we split at each newline and prepend the requested
        // indentation.

        // We create the sub-span starting just after the last newline, up to
        // and including the next. The index found marks the length of the
        // sub-span up to but excluding that newline. An otherwise empty
        // sub-span will cause indentation to be applied, followed by a newline.
        // This is intended behavior, as indentation may be arbitrary text such
        // as single-line comment tokens.
        var length = newLineIndex + 1;
        var start = 0;
        do
        {
            // We prepend the indentation before adding the line found.
            buffer.Add(indentation);
            var subSpan = value.Slice(start, length);
            buffer.Add(subSpan);

            // We determine the next sub-span start.
            start += length;
            if(start >= value.Length)
                break;

            // We scan the remaining characters for a newline.
            length = value[start..].IndexOf('\n') + 1;

            // If we are unable to locate another newline, we have reached
            // either the last line or we are terminating on an empty line. If
            // we have a non-empty line left, we need to append for one last
            // time.
            if(length == 0)
                length = value.Length - start;

        } while(length > 0);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(ReadOnlySpan<Char> customIndentation, ReadOnlySpan<Char> value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(customIndentation.Length == 0)
        {
            Render(value, ref buffer, indentation, cancellationToken);
            return;
        } else if(indentation.Length == 0)
        {
            Render(value, ref buffer, customIndentation, cancellationToken);
            return;
        }

        var combinedIndentationLength = customIndentation.Length + indentation.Length;
        var combinedIndentationArray = ArrayPool<Char>.Shared.Rent(combinedIndentationLength);
        try
        {
            var combinedIndentation = combinedIndentationArray.AsSpan(0, combinedIndentationLength);
            customIndentation.CopyTo(combinedIndentation);
            indentation.CopyTo(combinedIndentation[customIndentation.Length..]);
            Render(value, ref buffer, combinedIndentation, cancellationToken);
        } finally
        {
            ArrayPool<Char>.Shared.Return(combinedIndentationArray);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(Char value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(value != '\n')
            buffer.Add(indentation);

        buffer.Add(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(ReadOnlySpan<Char> customIndentation, Char value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(value != '\n')
        {
            buffer.Add(indentation);
            buffer.Add(customIndentation);
        }

        buffer.Add(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(String value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken) => Render(value.AsSpan(), ref buffer, indentation, cancellationToken);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(ReadOnlySpan<Char> customIndentation, String value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken) => Render(customIndentation, value.AsSpan(), ref buffer, indentation, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render<T>(in T value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
        where T : ITemplate
    {
        cancellationToken.ThrowIfCancellationRequested();
        value.Render(ref buffer, indentation, cancellationToken);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render<T>(ReadOnlySpan<Char> customIndentation, in T value, ref DynamicallyAllocatedBuffer<Char> buffer, ReadOnlySpan<Char> indentation, CancellationToken cancellationToken)
        where T : ITemplate
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(customIndentation.Length == 0)
        {
            value.Render(ref buffer, indentation, cancellationToken);
            return;
        } else if(indentation.Length == 0)
        {
            value.Render(ref buffer, customIndentation, cancellationToken);
            return;
        }

        var combinedIndentationLength = customIndentation.Length + indentation.Length;
        var combinedIndentationArray = ArrayPool<Char>.Shared.Rent(combinedIndentationLength);
        try
        {
            var combinedIndentation = combinedIndentationArray.AsSpan(0, combinedIndentationLength);
            customIndentation.CopyTo(combinedIndentation);
            indentation.CopyTo(combinedIndentation[customIndentation.Length..]);
            value.Render(ref buffer, combinedIndentation, cancellationToken);
        } finally
        {
            ArrayPool<Char>.Shared.Return(combinedIndentationArray);
        }
    }
}
