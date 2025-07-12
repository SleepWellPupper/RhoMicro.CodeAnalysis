// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>code-block-body-child</c> production.
/// </summary>
internal sealed record CodeBlockBodyChildSyntax : ISyntax
{
    private enum RepresentedType { Text, RenderBlock }

    private CodeBlockBodyChildSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "code-block-body-child";
    private readonly RepresentedType _representedType;
    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>text</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsText => _representedType == RepresentedType.Text;
    /// <summary>
    /// Gets the <c>text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public TextSyntax? AsText => Child as TextSyntax;
    /// <summary>
    /// Attempts to get the <c>text</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="text">
    /// Upon returning, contains the <c>text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>text</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsText([NotNullWhen(true)] out TextSyntax? text)
    {
        if(Child is TextSyntax t)
        {
            text = t;
            return true;
        }

        text = null;
        return false;
    }
    public CodeBlockBodyChildSyntax(TextSyntax child) : this(RepresentedType.Text, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>render-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsRenderBlock => _representedType == RepresentedType.RenderBlock;
    /// <summary>
    /// Gets the <c>render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public RenderBlockSyntax? AsRenderBlock => Child as RenderBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>render-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="valueBlock">
    /// Upon returning, contains the <c>render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>render-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsRenderBlock([NotNullWhen(true)] out RenderBlockSyntax? valueBlock)
    {
        if(Child is RenderBlockSyntax v)
        {
            valueBlock = v;
            return true;
        }

        valueBlock = null;
        return false;
    }
    public CodeBlockBodyChildSyntax(RenderBlockSyntax child) : this(RepresentedType.RenderBlock, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(CodeBlockBodyChildSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
