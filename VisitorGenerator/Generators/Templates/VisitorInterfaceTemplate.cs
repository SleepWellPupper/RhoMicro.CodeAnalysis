namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Provides base abstraction of the visitor pattern over nodes 
    /// as defined by (:model.Cref():).
    /// </summary>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) interface (:model.TypeNames().VisitorInterface:)
    {
    {:
        foreach(var node in model.LeafNodes())
        {
    :}
        /// <summary>
        /// Visits a node of type (:model.Cref():).
        /// </summary>
        /// <param name="target">
        /// The node to visit.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token used to request visiting to be cancelled.
        /// </param>
    {:

            (:Space4, new MethodSignatureTemplate("void", node.MemberNames().VisitMethod, node, true):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct VisitorInterfaceTemplate(BaseNodeModel model);
