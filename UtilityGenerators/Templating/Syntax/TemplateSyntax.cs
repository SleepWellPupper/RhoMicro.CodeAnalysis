// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>template</c> production.
/// </summary>
internal sealed record TemplateSyntax : ISyntax
{
    private enum RepresentedType { NotEmptyTemplate, EmptyTemplate }

    private TemplateSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "template";

    private readonly RepresentedType _representedType;

    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>not-empty-template</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsNotEmptyTemplate => _representedType == RepresentedType.NotEmptyTemplate;
    /// <summary>
    /// Gets the <c>not-empty-template</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public NotEmptyTemplateSyntax? AsNotEmptyTemplate => Child as NotEmptyTemplateSyntax;
    /// <summary>
    /// Attempts to get the <c>not-empty-template</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>not-empty-template</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>not-empty-template</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsNotEmptyTemplate([NotNullWhen(true)] out NotEmptyTemplateSyntax? syntax)
    {
        if(Child is NotEmptyTemplateSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TemplateSyntax(NotEmptyTemplateSyntax child) : this(RepresentedType.NotEmptyTemplate, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>empty-template</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsEmptyTemplate => _representedType == RepresentedType.EmptyTemplate;
    /// <summary>
    /// Gets the <c>empty-template</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public EmptyTemplateSyntax? AsEmptyTemplate => Child as EmptyTemplateSyntax;
    /// <summary>
    /// Attempts to get the <c>empty-template</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>empty-template</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>empty-template</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsEmptyTemplate([NotNullWhen(true)] out EmptyTemplateSyntax? syntax)
    {
        if(Child is EmptyTemplateSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TemplateSyntax(EmptyTemplateSyntax child) : this(RepresentedType.EmptyTemplate, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(TemplateSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
