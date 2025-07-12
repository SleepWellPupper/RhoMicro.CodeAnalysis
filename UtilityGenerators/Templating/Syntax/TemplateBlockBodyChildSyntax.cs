// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>template-block-body-child</c> production.
/// </summary>
internal sealed record TemplateBlockBodyChildSyntax : ISyntax
{
    private enum RepresentedType { Text, RenderBlock, CodeBlock }

    private TemplateBlockBodyChildSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "template-block-body-child";

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
    /// <param name="syntax">
    /// Upon returning, contains the <c>text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>text</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsText([NotNullWhen(true)] out TextSyntax? syntax)
    {
        if(Child is TextSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TemplateBlockBodyChildSyntax(TextSyntax child) : this(RepresentedType.Text, child) { }

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
    /// <param name="syntax">
    /// Upon returning, contains the <c>render-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>render-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsRenderBlock([NotNullWhen(true)] out RenderBlockSyntax? syntax)
    {
        if(Child is RenderBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TemplateBlockBodyChildSyntax(RenderBlockSyntax child) : this(RepresentedType.RenderBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>code-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsCodeBlock => _representedType == RepresentedType.CodeBlock;
    /// <summary>
    /// Gets a the <c>code-block</c> part of the production being represented; or <see
    /// langword="null"/> if the <c>render-block</c> part is being represented.
    /// </summary>
    public CodeBlockSyntax? AsCodeBlock => Child as CodeBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>code-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>code-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>code-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsCodeBlock([NotNullWhen(true)] out CodeBlockSyntax? syntax)
    {
        if(Child is CodeBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TemplateBlockBodyChildSyntax(CodeBlockSyntax child) : this(RepresentedType.CodeBlock, child) { }

    public override Int32 GetHashCode() =>
        HashCode.Combine(_representedType, Child);
    public Boolean Equals(TemplateBlockBodyChildSyntax other) =>
        other._representedType == _representedType
        && EqualityComparer<ISyntax>.Default.Equals(other.Child, Child);

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
