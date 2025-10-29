// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// Builds indented texts, primarily intended for C# source code.
/// The builder is indentation and newline aware.
/// </summary>
#if CSHARPSOURCEBUILDER_GENERATOR
[IncludeFile]
#endif
internal partial class CSharpSourceBuilder : IDisposable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="options">
    /// The options to use.
    /// </param>
    public CSharpSourceBuilder(CSharpSourceBuilderOptions options)
    {
        Options = options;
        _rentedArrayLifetime = new(options.CharBufferOwner);
        _buffer = options.CharBufferOwner.Rent(8);
        _indentations = [.. options.InitialIndentation];
        _indentationDetectorContext = new InterpolationIndentationDetectorContext(options, _rentedArrayLifetime);
        _detector = options.InitialInterpolationIndentationDetector;
    }

    /// <summary>
    /// Initializes a new instance, using <see cref="CSharpSourceBuilderOptions.Default"/>.
    /// </summary>
    public CSharpSourceBuilder() : this(CSharpSourceBuilderOptions.Default)
    {
    }

    private const Int32 AppendMethodHighOverloadPriority = 3;
    private const Int32 AppendMethodMediumOverloadPriority = 2;
    private const Int32 AppendMethodLowOverloadPriority = 1;
    private const Int32 AppendMethodNoOverloadPriority = 0;

    private readonly List<ReadOnlyMemory<Char>> _indentations;
    private readonly RentedArrayLifetime _rentedArrayLifetime;
    private readonly InterpolationIndentationDetectorContext _indentationDetectorContext;

    /// <summary>
    /// Gets the options used by the builder.
    /// </summary>
    public CSharpSourceBuilderOptions Options { get; }

    /// <summary>
    /// Gets the capacity (size) of the builder.
    /// </summary>
    public Int32 Capacity => _buffer.Length;

    /// <summary>
    /// Gets the amount of characters written to the builder so far.
    /// </summary>
    public Int32 Length { get; private set; }

    /// <summary>
    /// Gets the amount of newlines written to the builder.
    /// </summary>
    /// <remarks>
    /// The value returned may not correspond to the amount of lines written to the builder.
    /// For example, the following string:
    /// <code>
    /// "foo\nbar\nbaz"
    /// </code>
    /// will result in <see cref="Lines"/> returning <c>2</c> (as 2 newlines will have
    /// been written, but intuitively, the number of lines would be <c>3</c>.
    /// </remarks>
    public Int32 Lines { get; private set; }

    private IInterpolationIndentationDetector _detector;
    private Boolean _lastWasNewLine = true;
    private Boolean _lastWasEmptyLine = false;
    private Boolean _preludeWritten = false;
    private Char[] _buffer;
    private Boolean _appendCondition = true;
    private CancellationToken _cancellationToken;

    private static Int32 RoundUpToPowerOf2(Int32 value)
    {
        if (value >= 0x40000000)
        {
            return Int32.MaxValue;
        }

        if (value < 1)
        {
            return 1;
        }

        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
    }

    private Memory<Char> GetMemory(Int32 lengthHint)
    {
        var requiredLength = Length + lengthHint;

        if (_buffer.Length < requiredLength)
        {
            var newLength = RoundUpToPowerOf2(requiredLength);
            var newBuffer = Options.CharBufferOwner.Rent((Int32)newLength);
            _buffer.CopyTo(newBuffer);
            Options.CharBufferOwner.Return(_buffer);
            _buffer = newBuffer;
        }

        return _buffer.AsMemory(Length);
    }

    private Span<Char> GetSpan(Int32 lengthHint) => GetMemory(lengthHint).Span;

    private void AppendCore(ReadOnlySpan<Char> text, out ReadOnlyMemory<Char> lastLineStartText)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        lastLineStartText = default;

        if (text is [])
        {
            return;
        }

        TryWritePrelude();

        var tail = text;
        while (true)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var nextNewline = tail.IndexOfAny('\n', '\r');
            if (nextNewline is -1)
            {
                if (tail.Length is 0)
                {
                    break;
                }

                TryWriteIndentation();
                tail.CopyTo(GetSpan(tail.Length));
                lastLineStartText = _buffer.AsMemory(Length, tail.Length);

                Length += tail.Length;

                break;
            }

            var head = tail[..nextNewline];
            if (head.Length is not 0)
            {
                TryWriteIndentation();
            }

            head.CopyTo(GetSpan(head.Length));
            Length += head.Length;
            AppendLine();
            lastLineStartText = default;

            tail = tail[nextNewline..];

            if (tail is ['\r', '\n', ..])
            {
                tail = tail[2..];
            }
            else if (tail is [_, ..])
            {
                tail = tail[1..];
            }
            else
            {
                tail = [];
            }
        }
    }

    private void TryWritePrelude()
    {
        if (_preludeWritten)
        {
            return;
        }

        _preludeWritten = true;
        Options.Prelude.Invoke(this, _cancellationToken);
    }

    private void TryWriteIndentation()
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_lastWasNewLine)
        {
            return;
        }

        _lastWasNewLine = false;
        _lastWasEmptyLine = false;

        foreach (var indentation in _indentations)
        {
            AppendCore(indentation.Span, out _);
        }
    }

    /// <summary>
    /// Clears the builders contents.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Clear()
    {
        Length = 0;
        _preludeWritten = false;
        return this;
    }

    /// <summary>
    /// Begins a conditional scope.
    /// Methods executed after this method returns will execute if the
    /// condition evaluates to <see langword="true"/>.
    /// </summary>
    /// <param name="condition">
    /// The condition to conditionalize operations against.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder SetCondition(Boolean condition)
    {
        _appendCondition = condition;
        return this;
    }

    /// <summary>
    /// Unsets the conditional scope.
    /// Methods executed after this method returns will no longer execute
    /// depending on the condition previously set.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder UnsetCondition()
    {
        _appendCondition = true;
        return this;
    }

#region Indentation

    /// <summary>
    /// Indents the builder.
    /// </summary>
    /// <param name="indentation">
    /// The indentation to add to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Indent(ReadOnlyMemory<Char> indentation)
    {
        if (!_appendCondition)
        {
            return this;
        }

        _indentations.Add(indentation);
        return this;
    }

    /// <summary>
    /// Indents the builder.
    /// </summary>
    /// <param name="indentation">
    /// The indentation to add to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Indent(String indentation) => Indent(indentation.AsMemory());

    /// <summary>
    /// Indents the builder using the default indentation.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Indent() => Indent(Options.DefaultIndentation);

    /// <summary>
    /// Detents the builder once.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Detent()
    {
        if (!_appendCondition)
        {
            return this;
        }

        if (_indentations.Count is not 0)
        {
            _indentations.RemoveAt(_indentations.Count - 1);
        }

        return this;
    }

#endregion

#region Append

    /// <summary>
    /// Appends a text to the builder.
    /// </summary>
    /// <param name="text">
    /// The text to append to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(String text) =>
            Append(text.AsSpan());

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(ReadOnlySpan<Char> text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_appendCondition)
        {
            return this;
        }

        AppendCore(text, out _);
        return this;
    }

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(Char text) =>
            Append([text]);

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(AppendMethodMediumOverloadPriority)]
    public CSharpSourceBuilder Append(
            [InterpolatedStringHandlerArgument("")]
            InterpolatedStringHandler text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_appendCondition)
        {
            return this;
        }

        // handler performs writing operations
        text.Dispose();
        return this;
    }

    /// <inheritdoc cref="Append(ICSharpSourceComponent)"/>
    /// <typeparam name="T">
    /// The type of component to append,
    /// </typeparam>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append<T>(T component)
            where T : ICSharpSourceComponent
    {
        _cancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, _cancellationToken);

        return this;
    }

    /// <summary>
    /// Appends a component to the builder.
    /// </summary>
    /// <param name="component">
    /// The component to append to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    [OverloadResolutionPriority(AppendMethodNoOverloadPriority)]
    public CSharpSourceBuilder Append(ICSharpSourceComponent component)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, _cancellationToken);

        return this;
    }

#endregion

#region AppendLine

    /// <summary>
    /// Appends a newline to the builder
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    [OverloadResolutionPriority(AppendMethodHighOverloadPriority)]
    public CSharpSourceBuilder AppendLine()
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_appendCondition)
        {
            return this;
        }

        if (Options.Newline.Length is 0)
        {
            return this;
        }

        if (_lastWasEmptyLine && !Options.AllowConsecutiveEmptyLines)
        {
            return this;
        }

        TryWritePrelude();

        var buffer = GetSpan(Options.Newline.Length);
        Options.Newline.Span.CopyTo(buffer);
        Length += Options.Newline.Length;
        Lines++;
        _lastWasEmptyLine = _lastWasNewLine;
        _lastWasNewLine = true;

        return this;
    }

    /// <summary>
    /// Appends a text to the builder, followed by a newline.
    /// </summary>
    /// <param name="text">
    /// The text to append to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(String text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(ReadOnlySpan<Char> text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(Char text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(AppendMethodMediumOverloadPriority)]
    public CSharpSourceBuilder AppendLine(
            [InterpolatedStringHandlerArgument("")]
            InterpolatedStringHandler text)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_appendCondition)
        {
            return this;
        }

        // handler performs writing operations
        text.Dispose();
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(ICSharpSourceComponent)"/>
    /// <typeparam name="T">
    /// The type of component to append,
    /// </typeparam>
    [OverloadResolutionPriority(AppendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine<T>(T component)
            where T : ICSharpSourceComponent
    {
        _cancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, _cancellationToken);
        AppendLine();

        return this;
    }

    /// <summary>
    /// Appends a component to the builder, followed by a newline.
    /// </summary>
    /// <param name="component">
    /// The component to append to the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    [OverloadResolutionPriority(AppendMethodNoOverloadPriority)]
    public CSharpSourceBuilder AppendLine(ICSharpSourceComponent component)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, _cancellationToken);
        AppendLine();

        return this;
    }

#endregion

#region Append Type Name

    /// <inheritdoc cref="AppendTypeName(Type)"/>
    /// <typeparam name="T">
    /// The type whose name to append.
    /// </typeparam>
    public CSharpSourceBuilder AppendTypeName<T>()
    {
        AppendTypeName(typeof(T));

        return this;
    }

    /// <summary>
    /// Appends the globally qualified C# name of a type to the builder.
    /// </summary>
    /// <param name="type">
    /// The type whose name to append.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder AppendTypeName(Type type)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (!_appendCondition)
        {
            return this;
        }

        Append("global::");

        if (type.Namespace is { } ns)
        {
            Append(ns);
            Append('.');
        }

        var firstIllegalIndex = type.Name.AsSpan().IndexOfAny('`', '[');
        var length = firstIllegalIndex is -1 ? type.Name.Length : firstIllegalIndex;
        var name = type.Name.AsSpan(0, length);

        Append(name);

        if (type.IsArray)
        {
            Append("[]");
        }

        if (type.IsGenericType)
        {
            Append('<');

            foreach (var parameter in type.GetGenericArguments())
            {
                _cancellationToken.ThrowIfCancellationRequested();

                AppendTypeName(parameter);
            }

            Append('>');
        }

        return this;
    }

#endregion
    
#region Cancellation

    /// <summary>
    /// Sets the ambient cancellation token used by other methods in this builder.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        return this;
    }

    /// <summary>
    /// Unsets the ambient cancellation token used by other methods in this builder.
    /// After calling this method, cancellation will no longer be checked.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder UnsetCancellationToken()
    {
        _cancellationToken = default;
        return this;
    }

#endregion

#region Interpolation Indentation Detector

    /// <summary>
    /// Sets the indentation detector used by the builder.
    /// </summary>
    /// <param name="detector">
    /// The detector to use.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder SetInterpolationIndentationDetector(
            IInterpolationIndentationDetector detector)
    {
        if (!_appendCondition)
        {
            return this;
        }

        _detector = detector;

        return this;
    }

    /// <summary>
    /// Sets the indentation detector used by the builder.
    /// </summary>
    /// <param name="detector">
    /// The detector to use.
    /// </param>
    /// <param name="previousDetector">
    /// Upon returning, contains the previously used indentation detector.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder SetInterpolationIndentationDetector(
            IInterpolationIndentationDetector detector,
            out IInterpolationIndentationDetector previousDetector)
    {
        previousDetector = _detector;

        if (!_appendCondition)
        {
            return this;
        }

        _detector = detector;

        return this;
    }

#endregion

    /// <summary>
    /// Builds the source string.
    /// </summary>
    /// <returns>
    /// The source string.
    /// </returns>
    public override String ToString()
    {
        TryWritePrelude();
        var result = new String(_buffer, 0, Length);

        return result;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _rentedArrayLifetime.Dispose();
        _buffer = [];
    }
}
