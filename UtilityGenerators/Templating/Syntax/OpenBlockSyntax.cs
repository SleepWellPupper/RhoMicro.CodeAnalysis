// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>open-block</c> production.
/// </summary>
internal sealed record OpenBlockSyntax : ISyntax
{
    private enum RepresentedType { OpenCodeBlock, OpenRenderBlock, OpenTemplateBlock }

    private OpenBlockSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "open-block";

    private readonly RepresentedType _representedType;

    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>open-code-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsOpenCodeBlock => _representedType == RepresentedType.OpenCodeBlock;
    /// <summary>
    /// Gets the <c>open-code-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public OpenCodeBlockSyntax? AsOpenCodeBlock => Child as OpenCodeBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>open-code-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>open-code-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>open-code-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsOpenCodeBlock([NotNullWhen(true)] out OpenCodeBlockSyntax? syntax)
    {
        if(Child is OpenCodeBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public OpenBlockSyntax(OpenCodeBlockSyntax child) : this(RepresentedType.OpenCodeBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>open-render-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsOpenRenderBlock => _representedType == RepresentedType.OpenRenderBlock;
    /// <summary>
    /// Gets the <c>open-render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public OpenRenderBlockSyntax? AsOpenRenderBlock => Child as OpenRenderBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>open-render-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>open-render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>open-render-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsOpenRenderBlock([NotNullWhen(true)] out OpenRenderBlockSyntax? syntax)
    {
        if(Child is OpenRenderBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public OpenBlockSyntax(OpenRenderBlockSyntax child) : this(RepresentedType.OpenRenderBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>open-template-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsOpenTemplateBlock => _representedType == RepresentedType.OpenTemplateBlock;
    /// <summary>
    /// Gets the <c>open-template-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public OpenTemplateBlockSyntax? AsOpenTemplateBlock => Child as OpenTemplateBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>open-template-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>open-template-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>open-template-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsOpenTemplateBlock([NotNullWhen(true)] out OpenTemplateBlockSyntax? syntax)
    {
        if(Child is OpenTemplateBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public OpenBlockSyntax(OpenTemplateBlockSyntax child) : this(RepresentedType.OpenTemplateBlock, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(OpenBlockSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}

