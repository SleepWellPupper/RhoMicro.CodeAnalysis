namespace RhoMicro.CodeAnalysis.Templating.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>code-body-child</c> production.
/// </summary>
internal sealed record CodeBodyChildSyntax : ISyntax, IEquatable<CodeBodyChildSyntax>
{
    private enum RepresentedType { Text, RenderBlock }
    
    private CodeBodyChildSyntax(RepresentedType representedType, Object part)
    {
        _representedType = representedType;
        _syntax = part;
    }
    
    private readonly RepresentedType _representedType;
    private readonly Object _syntax;

    /// <summary>
    /// Gets a value indicating whether the <c>text</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsText => _representedType == RepresentedType.Text;
    /// <summary>
    /// Gets a the <c>text</c> part of the production being represented; or <see
    /// langword="null"/> if the <c>render-block</c> part is being represented.
    /// </summary>
    public TextSyntax? AsText => _syntax as TextSyntax;
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
        if(_syntax is TextSyntax t)
        {
            text = t;
            return true;
        }

        text = null;
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether the <c>render-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsRenderBlock => _representedType == RepresentedType.RenderBlock;
    /// <summary>
    /// Gets a the <c>render-block</c> part of the production being represented; or <see
    /// langword="null"/> if the <c>text</c> part is being represented.
    /// </summary>
    public RenderBlockSyntax? AsRenderBlock => _syntax as RenderBlockSyntax;
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
        if(_syntax is RenderBlockSyntax v)
        {
            valueBlock = v;
            return true;
        }

        valueBlock = null;
        return false;
    }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public static implicit operator CodeBodyChildSyntax(TextSyntax text) => new(RepresentedType.Text, text);
    public static implicit operator CodeBodyChildSyntax(RenderBlockSyntax valueBlock) => new(RepresentedType.RenderBlock, valueBlock);

    public Boolean Equals(CodeBodyChildSyntax other) => _representedType == other._representedType && EqualityComparer<Object>.Default.Equals(_syntax, other._syntax);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, _syntax);
    public override String ToString() => this.ToAstString();
}
