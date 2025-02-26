namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>escaped-text</c> production.
/// </summary>
internal sealed record EscapedTextSyntax : ISyntax
{
    private enum RepresentedType { EscapedOpenBlock, EscapedCloseBlock, EmptyBlock }

    private EscapedTextSyntax(RepresentedType representedType, ISyntax part)
    {
        _representedType = representedType;
        Child = part;
    }

    public const String Production = "escaped-text";

    private readonly RepresentedType _representedType;
    public ISyntax Child { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>escaped-open-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsEscapedOpenBlock => _representedType == RepresentedType.EscapedOpenBlock;
    /// <summary>
    /// Gets the <c>escaped-open-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public EscapedOpenBlockSyntax? AsEscapedOpenBlock => Child as EscapedOpenBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>escaped-open-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>escaped-open-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>escaped-open-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsEscapedOpenBlock([NotNullWhen(true)] out EscapedOpenBlockSyntax? syntax)
    {
        if(Child is EscapedOpenBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public EscapedTextSyntax(EscapedOpenBlockSyntax child) : this(RepresentedType.EscapedOpenBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>escaped-close-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsEscapedCloseBlock => _representedType == RepresentedType.EscapedCloseBlock;
    /// <summary>
    /// Gets the <c>escaped-close-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public EscapedCloseBlockSyntax? AsEscapedCloseBlock => Child as EscapedCloseBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>escaped-close-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>escaped-close-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>escaped-close-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsEscapedCloseBlock([NotNullWhen(true)] out EscapedCloseBlockSyntax? syntax)
    {
        if(Child is EscapedCloseBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public EscapedTextSyntax(EscapedCloseBlockSyntax child) : this(RepresentedType.EscapedCloseBlock, child) { }

    /// <summary>
    /// Gets a value indicating whether the <c>empty-block</c> part of the production
    /// is being represented.
    /// </summary>
    public Boolean IsEmptyBlock => _representedType == RepresentedType.EmptyBlock;
    /// <summary>
    /// Gets the <c>empty-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </summary>
    public EmptyBlockSyntax? AsEmptyBlock => Child as EmptyBlockSyntax;
    /// <summary>
    /// Attempts to get the <c>empty-block</c> part of the production being
    /// represented.
    /// </summary>
    /// <param name="syntax">
    /// Upon returning, contains the <c>empty-block</c> part of the production if it is
    /// being represented; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <c>empty-block</c> part of the production is
    /// being represented; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean TryAsEmptyBlock([NotNullWhen(true)] out EmptyBlockSyntax? syntax)
    {
        if(Child is EmptyBlockSyntax s)
        {
            syntax = s;
            return true;
        }

        syntax = null;
        return false;
    }
    public EscapedTextSyntax(EmptyBlockSyntax child) : this(RepresentedType.EmptyBlock, child) { }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);

    public Boolean Equals(EscapedTextSyntax other) => _representedType == other._representedType && EqualityComparer<ISyntax>.Default.Equals(Child, other.Child);
    public override Int32 GetHashCode() => HashCode.Combine(_representedType, Child);

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}