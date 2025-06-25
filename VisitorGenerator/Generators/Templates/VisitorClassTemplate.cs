namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Implements the visitor pattern as defined by <see cref="(:model.TypeNames().VisitorInterfaceFull:)"/>.
    /// </summary>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) abstract class (:model.TypeNames().VisitorClass:): (:model.TypeNames().VisitorInterfaceFull:)
    {
    {:
        foreach(var node in model.LeafNodes())
        {
            (:Space4, new VisitorClassMethodGroupTemplate(node):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct VisitorClassTemplate(BaseNodeModel model);
