// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;
/// <summary>
/// Represents the <c>not-escaped-text-child</c> production.
/// </summary>
internal sealed record NotEscapedTextChildSyntax : ISyntax
{
    private enum RepresentedType { NotNewline, Newline, Whitespaces }

    private NotEscapedTextChildSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "not-escaped-text-child";

    private readonly RepresentedType _representedType;
    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>not-newline</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsNotNewline => _representedType == RepresentedType.NotNewline;
    /// <summary>
    /// Gets the <c>not-newline</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public NotNewlineSyntax? AsNotNewline => Child as NotNewlineSyntax;
    /// <summary>
    /// Attempts to get the <c>not-newline</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>not-newline</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>not-newline</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsNotNewline([NotNullWhen(true)] out NotNewlineSyntax? syntax)
    {
        if(Child is NotNewlineSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public NotEscapedTextChildSyntax(NotNewlineSyntax child) : this(RepresentedType.NotNewline, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>whitespaces</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsWhitespaces => _representedType == RepresentedType.Whitespaces;
    /// <summary>
    /// Gets the <c>whitespaces</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public WhitespacesSyntax? AsWhitespaces => Child as WhitespacesSyntax;
    /// <summary>
    /// Attempts to get the <c>whitespaces</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>whitespaces</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>whitespaces</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsWhitespaces([NotNullWhen(true)] out WhitespacesSyntax? syntax)
    {
        if(Child is WhitespacesSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public NotEscapedTextChildSyntax(WhitespacesSyntax child) : this(RepresentedType.Whitespaces, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>newline</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsNewline => _representedType == RepresentedType.Newline;
    /// <summary>
    /// Gets the <c>newline</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public NewlineSyntax? AsNewline => Child as NewlineSyntax;
    /// <summary>
    /// Attempts to get the <c>newline</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>newline</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>newline</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsNewline([NotNullWhen(true)] out NewlineSyntax? syntax)
    {
        if(Child is NewlineSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public NotEscapedTextChildSyntax(NewlineSyntax child) : this(RepresentedType.Newline, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(NotEscapedTextChildSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
