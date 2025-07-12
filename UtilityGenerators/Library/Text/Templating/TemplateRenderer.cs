// SPDX-License-Identifier: MPL-2.0

#pragma warning disable IDE0251 // Make member 'readonly' => would cause buffer
// operations to only be executed on copy
namespace RhoMicro.CodeAnalysis.Library.Text.Templating;
/// <summary>
/// Provides the context for rendering templates.
/// </summary>
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal ref partial struct TemplateRenderer : IDisposable
{
    public TemplateRenderer(Span<Char> initialBuffer, Span<Char> initialIndentationBuffer, CancellationToken cancellationToken = default)
    {
        _buffer = new(initialBuffer);
        _indentationBuffer = new(initialIndentationBuffer);
        _cancellationToken = cancellationToken;
    }

    private readonly CancellationToken _cancellationToken;
    private DynamicallyAllocatedCharBuffer _indentationBuffer;
    private DynamicallyAllocatedCharBuffer _buffer;

    /// <summary>
    /// Gets the indentation currently applied to the context.
    /// </summary>
    public ReadOnlySpan<Char> Indentation => _indentationBuffer.Span;

    /// <summary>
    /// Adds indentation to the context.
    /// </summary>
    /// <param name="indentation">
    /// The indentation to append.
    /// </param>
    public void Indent(ReadOnlySpan<Char> indentation) => _indentationBuffer.Add(indentation);
    /// <summary>
    /// Detents the context by the amount of characters specified.
    /// </summary>
    /// <param name="characters">
    /// The amount of characters to detent by.
    /// </param>
    public void Detent(Int32 characters) => _indentationBuffer.Retreat(characters);

    /// <inheritdoc/>
    public void Dispose()
    {
        _buffer.Dispose();
        _indentationBuffer.Dispose();
    }

    /// <inheritdoc/>
    public override String ToString() => _buffer.ToString();
}

