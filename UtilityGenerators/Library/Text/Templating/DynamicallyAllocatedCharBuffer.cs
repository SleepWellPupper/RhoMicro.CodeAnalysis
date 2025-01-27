namespace RhoMicro.CodeAnalysis.Library.Text.Templating;

using System;
using System.Buffers;
using System.Numerics;

/// <summary>
/// Represents a dynamically allocated buffer of characters that may be
/// initially allocated on the stack and promoted to a heap allocated buffer if
/// needed. The allocated buffer will be rented from <see
/// cref="ArrayPool{T}.Shared"/> and returned upon disposal of the containing
/// <see cref="DynamicallyAllocatedCharBuffer"/> instance.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal ref struct DynamicallyAllocatedCharBuffer : IDisposable
{
    /// <summary>
    /// Allocates a new instance.
    /// </summary>
    /// <param name="initialBuffer">
    /// The initial buffer to use. This may be stack-allocated by the consumer.
    /// </param>
    public DynamicallyAllocatedCharBuffer(Span<Char> initialBuffer) => _span = initialBuffer;
    /// <summary>
    /// Allocates a new instance.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity of the buffer. This will heap-allocate a new
    /// buffer.
    /// </param>
    public DynamicallyAllocatedCharBuffer(Int32 initialCapacity) => EnsureLength(initialCapacity);

    private Char[]? _rented;
    private Span<Char> _span;
    private Int32 _cursor;

    /// <summary>
    /// Gets a span of the characters currently in the buffer.
    /// </summary>
    public readonly Span<Char> Span => _span[.._cursor];

    /// <summary>
    /// Adds a span of characters to the end of the buffer, resizing it if
    /// necessary. If the length required to accommodate the characters exceeds
    /// the current buffer, it will be resized. If the buffer was previously
    /// stack-allocated but needs resizing, a new, heap-allocated buffer is
    /// created.
    /// </summary>
    /// <param name="characters">
    /// The characters to add to the buffer.
    /// </param>
    public void Add(params ReadOnlySpan<Char> characters)
    {
        if(characters.Length == 0)
            return;

        var target = Reserve(characters.Length);
        characters.CopyTo(target);
    }
    /// <summary>
    /// Ensures that space for at least <paramref name="length"/> characters is
    /// available in the buffer and returns a span with exactly that length. The
    /// buffer is advanced by the length requested. The span returned may not be
    /// zeroed.
    /// </summary>
    /// <param name="length">
    /// The length to reserve.
    /// </param>
    /// <returns>
    /// A span with a length of exactly <paramref name="length"/>.
    /// </returns>
    public Span<Char> Reserve(Int32 length)
    {
        EnsureLength(length);
        var result = _span.Slice(_cursor, length);
        Advance(length);

        return result;
    }
    /// <summary>
    /// Clears the buffer.
    /// </summary>
    public void Clear() => Retreat(_cursor);
    /// <summary>
    /// retreats the cursor by the length specified.
    /// </summary>
    /// <param name="length">
    /// The length by which to retreat the buffer cursor.
    /// </param>
    public void Retreat(Int32 length) => _cursor -= length;
    private void Advance(Int32 length) => _cursor += length;
    private void EnsureLength(Int32 requiredLength)
    {
        var delta = _cursor + requiredLength - _span.Length;

        if(delta > 0)
        {
            var newSize = checked((Int32)BitOperations.RoundUpToPowerOf2((UInt32)( _span.Length + delta )));
            _rented = ArrayPool<Char>.Shared.Rent(newSize);
            Span<Char> newHeapAllocatedBuffer = _rented;
            _span.CopyTo(newHeapAllocatedBuffer);
            _span = newHeapAllocatedBuffer;
        }
    }

    /// <inheritdoc/>
    public readonly void Dispose()
    {
        if(_rented is not null)
            ArrayPool<Char>.Shared.Return(_rented);
    }

    public readonly override String ToString() => Span.ToString();
}
