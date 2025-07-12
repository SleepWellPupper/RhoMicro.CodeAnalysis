// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>close-block</c> production.
/// </summary>
internal sealed record CloseBlockSyntax : ISyntax
{
    private enum RepresentedType { CloseCodeBlock, CloseRenderBlock, CloseTemplateBlock }

    private CloseBlockSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "close-block";

    private readonly RepresentedType _representedType;

    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>close-code-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsCloseCodeBlock => _representedType == RepresentedType.CloseCodeBlock;
    /// <summary>
    /// Gets the <c>close-code-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public CloseCodeBlockSyntax? AsCloseCodeBlock => Child as CloseCodeBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>close-code-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>close-code-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>close-code-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsCloseCodeBlock([NotNullWhen(true)] out CloseCodeBlockSyntax? syntax)
    {
        if(Child is CloseCodeBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public CloseBlockSyntax(CloseCodeBlockSyntax child) : this(RepresentedType.CloseCodeBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>close-render-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsCloseRenderBlock => _representedType == RepresentedType.CloseRenderBlock;
    /// <summary>
    /// Gets the <c>close-render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public CloseRenderBlockSyntax? AsCloseRenderBlock => Child as CloseRenderBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>close-render-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>close-render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>close-render-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsCloseRenderBlock([NotNullWhen(true)] out CloseRenderBlockSyntax? syntax)
    {
        if(Child is CloseRenderBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public CloseBlockSyntax(CloseRenderBlockSyntax child) : this(RepresentedType.CloseRenderBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>close-template-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsCloseTemplateBlock => _representedType == RepresentedType.CloseTemplateBlock;
    /// <summary>
    /// Gets the <c>close-template-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public CloseTemplateBlockSyntax? AsCloseTemplateBlock => Child as CloseTemplateBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>close-template-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>close-template-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>close-template-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsCloseTemplateBlock([NotNullWhen(true)] out CloseTemplateBlockSyntax? syntax)
    {
        if(Child is CloseTemplateBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public CloseBlockSyntax(CloseTemplateBlockSyntax child) : this(RepresentedType.CloseTemplateBlock, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(CloseBlockSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
