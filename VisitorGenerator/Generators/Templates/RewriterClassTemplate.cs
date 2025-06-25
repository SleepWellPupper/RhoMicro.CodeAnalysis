namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Rewriter for nodes as defined by <see cref="(:model.TypeNames().RewriterInterfaceFull:)"/>.
    /// </summary>
    /// <remarks>
    /// No implementation for node traversal is provided. Implementers must ensure that node traversal is proeprly implemented.
    /// </remarks>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) abstract class (:model.TypeNames().RewriterClass:): (:model.TypeNames().RewriterInterfaceFull:)
    {
    {:
        foreach(var node in model.LeafNodes())
        {
            (:Space4, new RewriteMethodTemplate(node):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct RewriterClassTemplate(BaseNodeModel model);
