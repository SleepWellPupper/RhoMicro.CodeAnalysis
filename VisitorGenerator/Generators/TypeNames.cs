namespace RhoMicro.CodeAnalysis;

[NonEquatable]
internal readonly partial struct TypeNames(NodeSignatureModel signature)
{
    public TypeNameTemplate RewriterInterface { get; } = new("I", signature, "Rewriter", false);
    public TypeNameTemplate RewriterInterfaceFull { get; } = new("I", signature, "Rewriter", true);
    public TypeNameTemplate RewriterClass { get; } = new(String.Empty, signature, "Rewriter", false);

    public TypeNameTemplate VisitorInterface { get; } = new("I", signature, "Visitor", false);
    public TypeNameTemplate VisitorInterfaceFull { get; } = new("I", signature, "Visitor", true);
    public TypeNameTemplate VisitorClass { get; } = new(String.Empty, signature, "Visitor", false);

    public TypeNameTemplate GenericVisitorInterface { get; } = new("I", signature, "Visitor<TResult>", false);
    public TypeNameTemplate GenericVisitorInterfaceFull { get; } = new("I", signature, "Visitor<TResult>", true);
    public TypeNameTemplate GenericVisitorClass { get; } = new(String.Empty, signature, "Visitor<TResult>", false);
}
