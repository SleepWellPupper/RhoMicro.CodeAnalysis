namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a dynamically allocated buffer that may be initially allocated on
/// the stack and promoted to a heap allocated buffer if needed. The allocated
/// buffer will be rented from <see cref="ArrayPool{T}.Shared"/> and returned
/// upon disposal of the <see cref="DynamicallyAllocatedBuffer{T}"/> instance.
/// </summary>
/// <typeparam name="T">
/// The type of element managed.
/// </typeparam>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal ref struct DynamicallyAllocatedBuffer<T> : IDisposable
{
    /// <summary>
    /// Allocates a new instance.
    /// </summary>
    /// <param name="initialBuffer">
    /// The initial buffer to use. This may be stack-allocated by the consumer.
    /// </param>
    public DynamicallyAllocatedBuffer(Span<T> initialBuffer) => _span = initialBuffer;

    private T[]? _rented;
    private Span<T> _span;
    private Int32 _cursor;

    /// <summary>
    /// Gets a span of the values currently in the buffer.
    /// </summary>
    public Span<T> Span => _span[.._cursor];

    /// <summary>
    /// Adds an element to the end of the buffer, resizing it if necessary.
    /// If the length required to accommodate the element exceeds the
    /// current buffer, it will be resized. If the buffer was previously
    /// stack-allocated but needs resizing, a new, heap-allocated buffer is
    /// created.
    /// </summary>
    /// <param name="element">
    /// The element to add to the buffer.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T element)
    {
        EnsureLength(requiredLength: 1);
        _span[_cursor] = element;
        Advance(1);
    }
    /// <summary>
    /// Adds a span of elements to the end of the buffer, resizing it if necessary.
    /// If the length required to accommodate the elements exceeds the
    /// current buffer, it will be resized. If the buffer was previously
    /// stack-allocated but needs resizing, a new, heap-allocated buffer is
    /// created.
    /// </summary>
    /// <param name="elements">
    /// The elements to add to the buffer.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(ReadOnlySpan<T> elements)
    {
        var target = Reserve(elements.Length);
        elements.CopyTo(target);
    }
    /// <summary>
    /// Ensures that space for at least <paramref name="length"/> elements is
    /// available in the buffer and returns a span with exactly that length.
    /// The buffer is advanced by the length requested.
    /// The span returned may not be zeroed.
    /// </summary>
    /// <param name="length">
    /// The length to reserve.
    /// </param>
    /// <returns>
    /// A span with a length of exactly <paramref name="length"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Reserve(Int32 length)
    {
        EnsureLength(length);
        var result = _span.Slice(_cursor, length);
        Advance(length);

        return result;
    }
    /// <summary>
    /// Clears the buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Retreat(_cursor);
    /// <summary>
    /// retreats the cursor by the length specified.
    /// </summary>
    /// <param name="length">
    /// The length by which to retreat the buffer cursor.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Retreat(Int32 length) => _cursor -= length;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(Int32 length) => _cursor += length;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureLength(Int32 requiredLength)
    {
        var delta = _cursor + requiredLength - _span.Length;

        if(delta > 0)
        {
            var newSize = checked((Int32)BitOperations.RoundUpToPowerOf2((UInt32)( _span.Length + delta )));
            _rented = ArrayPool<T>.Shared.Rent(newSize);
            Span<T> newHeapAllocatedBuffer = _rented;
            _span.CopyTo(newHeapAllocatedBuffer);
            _span = newHeapAllocatedBuffer;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if(_rented is not null)
            ArrayPool<T>.Shared.Return(_rented);
    }
}
