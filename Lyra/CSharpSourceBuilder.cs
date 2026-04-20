// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Lyra;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    private const Int32 _appendMethodHighOverloadPriority = 3;
    private const Int32 _appendMethodMediumOverloadPriority = 2;
    private const Int32 _appendMethodLowOverloadPriority = 1;
    private const Int32 _appendMethodNoOverloadPriority = 0;

    private readonly Stack<Boolean> _appendConditions = [];
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

    private Boolean IsAppendConditionMet => _appendConditions.Count == 0 || _appendConditions.Peek();

    private IInterpolationIndentationDetector _detector;
    private Boolean _lastWasNewLine = true;
    private Boolean _lastWasEmptyLine = false;
    private Boolean _preludeWritten = false;
    private Char[] _buffer;
    public CancellationToken CancellationToken { get; private set; }

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
            var newBuffer = Options.CharBufferOwner.Rent(newLength);
            _buffer.CopyTo(newBuffer.AsSpan());
            Options.CharBufferOwner.Return(_buffer);
            _buffer = newBuffer;
        }

        return _buffer.AsMemory(Length);
    }

    private Span<Char> GetSpan(Int32 lengthHint) => GetMemory(lengthHint).Span;

    private void AppendCore(ReadOnlySpan<Char> text, out ReadOnlyMemory<Char> lastLineStartText)
    {
        CancellationToken.ThrowIfCancellationRequested();

        lastLineStartText = default;

        if (text is [])
        {
            return;
        }

        TryWritePrelude();

        var tail = text;
        while (true)
        {
            CancellationToken.ThrowIfCancellationRequested();

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
        DetentAll(out var indentations);
        Options.Prelude.Invoke(this, CancellationToken);
        Indent(indentations);
    }

    private void TryWriteIndentation()
    {
        CancellationToken.ThrowIfCancellationRequested();

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
        _appendConditions.Push(condition);
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
        _appendConditions.Pop();
        return this;
    }

#region Indentation

    /// <summary>
    /// Detents the builder fully.
    /// </summary>
    /// <param name="indentations">
    /// The indentations removed from the builder.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder DetentAll(out ImmutableArray<ReadOnlyMemory<Char>> indentations)
    {
        if (!IsAppendConditionMet)
        {
            indentations = [];
            return this;
        }

        var result = new ReadOnlyMemory<Char>[_indentations.Count];
        for (var i = 0; i < _indentations.Count; i++)
        {
            result[i] = _indentations[i];
        }

        _indentations.Clear();

        indentations = ImmutableCollectionsMarshal.AsImmutableArray(result);
        return this;
    }

    /// <summary>
    /// Detents the builder fully.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder DetentAll()
    {
        if (!IsAppendConditionMet)
        {
            return this;
        }

        _indentations.Clear();

        return this;
    }

    /// <summary>
    /// Indents the builder.
    /// </summary>
    /// <param name="indentations">
    /// The indentations to add to the builder.
    /// </param>
    /// <typeparam name="TEnumerable">
    /// The type of enumerable containing the indentations.
    /// </typeparam>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder Indent<TEnumerable>(TEnumerable indentations)
        where TEnumerable : IEnumerable<ReadOnlyMemory<Char>>
    {
        if (!IsAppendConditionMet)
        {
            return this;
        }

        foreach (var indentation in indentations)
        {
            _indentations.Add(indentation);
        }

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
    public CSharpSourceBuilder Indent(ReadOnlyMemory<Char> indentation)
    {
        if (!IsAppendConditionMet)
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
        if (!IsAppendConditionMet)
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
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(String text) =>
        Append(text.AsSpan());

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(ReadOnlySpan<Char> text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (!IsAppendConditionMet)
        {
            return this;
        }

        AppendCore(text, out _);
        return this;
    }

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append(Char text) =>
        Append([text]);

    /// <inheritdoc cref="Append(string)"/>
    [OverloadResolutionPriority(_appendMethodMediumOverloadPriority)]
    public CSharpSourceBuilder Append(
        [InterpolatedStringHandlerArgument("")]
        InterpolatedStringHandler text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (!IsAppendConditionMet)
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
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder Append<T>(T component)
        where T : ICSharpSourceComponent
    {
        CancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, CancellationToken);

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
    [OverloadResolutionPriority(_appendMethodNoOverloadPriority)]
    public CSharpSourceBuilder Append(ICSharpSourceComponent component)
    {
        CancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, CancellationToken);

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
    [OverloadResolutionPriority(_appendMethodHighOverloadPriority)]
    public CSharpSourceBuilder AppendLine()
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (!IsAppendConditionMet)
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
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(String text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(ReadOnlySpan<Char> text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine(Char text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        Append(text);
        AppendLine();

        return this;
    }

    /// <inheritdoc cref="AppendLine(string)"/>
    [OverloadResolutionPriority(_appendMethodMediumOverloadPriority)]
    public CSharpSourceBuilder AppendLine(
        [InterpolatedStringHandlerArgument("")]
        InterpolatedStringHandler text)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (!IsAppendConditionMet)
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
    [OverloadResolutionPriority(_appendMethodLowOverloadPriority)]
    public CSharpSourceBuilder AppendLine<T>(T component)
        where T : ICSharpSourceComponent
    {
        CancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, CancellationToken);
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
    [OverloadResolutionPriority(_appendMethodNoOverloadPriority)]
    public CSharpSourceBuilder AppendLine(ICSharpSourceComponent component)
    {
        CancellationToken.ThrowIfCancellationRequested();

        component.AppendTo(this, CancellationToken);
        AppendLine();

        return this;
    }

#endregion

#region Append Type Name

    /// <inheritdoc cref="AppendTypeName{T}(TypeNameOptions)"/>
    public CSharpSourceBuilder AppendTypeName<T>() => AppendTypeName<T>(Options.DefaultTypeNameOptions);

    /// <inheritdoc cref="AppendTypeName(Type)"/>
    /// <typeparam name="T">
    /// The type whose name to append.
    /// </typeparam>
    public CSharpSourceBuilder AppendTypeName<T>(TypeNameOptions options)
    {
        AppendTypeName(typeof(T), options);

        return this;
    }

    /// <inheritdoc cref="AppendTypeName(Type, TypeNameOptions)"/>
    public CSharpSourceBuilder AppendTypeName(Type type) => AppendTypeName(type, Options.DefaultTypeNameOptions);

    /// <summary>
    /// Appends the globally qualified C# name of a type to the builder.
    /// </summary>
    /// <param name="options">
    /// The options to use when appending the type name.
    /// </param>
    /// <param name="type">
    /// The type whose name to append.
    /// </param>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder AppendTypeName(Type type, TypeNameOptions options)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (!IsAppendConditionMet)
        {
            return this;
        }

        if (options.UseTypeAliases && _aliasedTypes.TryGetValue(type, out var aliased))
        {
            Append(aliased);
        }
        else
        {
            AppendMetadataTypeName(type, options);
        }

        return this;
    }

    private static readonly Dictionary<Type, String> _aliasedTypes = new()
    {
        { typeof(Boolean), "bool" },
        { typeof(Byte), "byte" },
        { typeof(SByte), "sbyte" },
        { typeof(Int16), "short" },
        { typeof(UInt16), "ushort" },
        { typeof(Int32), "int" },
        { typeof(UInt32), "int" },
        { typeof(Int64), "long" },
        { typeof(UInt64), "long" },
        { typeof(Single), "float" },
        { typeof(Double), "double" },
        { typeof(Decimal), "decimal" },
        { typeof(Char), "char" },
        { typeof(String), "string" }
    };

    private void AppendMetadataTypeName(Type type, TypeNameOptions options)
    {
        if (options.UseGloballyQualifiedName)
        {
            Append("global::");

            if (type.Namespace is { } ns)
            {
                Append(ns);
                Append('.');
            }
        }

        var firstIllegalIndex = type.Name.AsSpan().IndexOfAny('`', '[');
        var length = firstIllegalIndex is -1 ? type.Name.Length : firstIllegalIndex;
        var name = type.Name.AsSpan(0, length);

        Append(name);

        if (type.IsArray)
        {
            Append("[]");
        }

        if (!type.IsGenericType)
        {
            return;
        }

        Append('<');

        for (var i = 0; i < type.GetGenericArguments().Length; i++)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (i is not 0)
            {
                Append(", ");
            }
            
            var parameter = type.GetGenericArguments()[i];
            AppendTypeName(parameter, options);
        }

        Append('>');
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
        CancellationToken = cancellationToken;
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
        CancellationToken = default;
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
        if (!IsAppendConditionMet)
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

        if (!IsAppendConditionMet)
        {
            return this;
        }

        _detector = detector;

        return this;
    }

#endregion

#region Miscellaneous

    /// <summary>
    /// Skips checks to append the prelude until <see cref="Clear"/> is called.
    /// This will cause the prelude to be omitted if no other calls appending
    /// text to the builder have been made.
    /// </summary>
    /// <returns>
    /// A reference to the builder, for chaining of further method calls.
    /// </returns>
    public CSharpSourceBuilder SkipPreludeChecks()
    {
        _preludeWritten = true;

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
