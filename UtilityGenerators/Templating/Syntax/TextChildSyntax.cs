namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>text-child</c> production.
/// </summary>
internal sealed record TextChildSyntax : ISyntax
{
    private enum RepresentedType { NotEscapedText, EscapedText }

    private TextChildSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "text-child";

    private readonly RepresentedType _representedType;

    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>not-escaped-text</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsNotEscapedText => _representedType == RepresentedType.NotEscapedText;
    /// <summary>
    /// Gets the <c>not-escaped-text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public NotEscapedTextSyntax? AsNotEscapedText => Child as NotEscapedTextSyntax;
    /// <summary>
    /// Attempts to get the <c>not-escaped-text</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>not-escaped-text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>not-escaped-text</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsNotEscapedText([NotNullWhen(true)] out NotEscapedTextSyntax? syntax)
    {
        if(Child is NotEscapedTextSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TextChildSyntax(NotEscapedTextSyntax child) : this(RepresentedType.NotEscapedText, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>escaped-text</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsEscapedText => _representedType == RepresentedType.EscapedText;
    /// <summary>
    /// Gets the <c>escaped-text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public EscapedTextSyntax? AsEscapedText => Child as EscapedTextSyntax;
    /// <summary>
    /// Attempts to get the <c>escaped-text</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>escaped-text</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>escaped-text</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsEscapedText([NotNullWhen(true)] out EscapedTextSyntax? syntax)
    {
        if(Child is EscapedTextSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public TextChildSyntax(EscapedTextSyntax child) : this(RepresentedType.EscapedText, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(TextChildSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}