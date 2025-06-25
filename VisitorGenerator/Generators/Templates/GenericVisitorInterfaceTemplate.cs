namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Provides base abstraction of the visitor pattern over nodes 
    /// as defined by (:model.Cref():). Visitors 
    /// of this type return some result from their visit.
    /// </summary>
    /// <typeparam name="TResult">The type of result produced by visiting nodes.</typeparam>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) interface (:model.TypeNames().GenericVisitorInterface:)
    {
    {:
        foreach(var node in model.LeafNodes())
        {
    :}
        /// <summary>
        /// Visits a node of type <see cref="(:model.FullName():)"/>, 
        /// producing a result of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="target">
        /// The node to visit.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token used to request visiting to be cancelled.
        /// </param>
        /// <returns>
        /// The result of the visit, of type <typeparamref name="TResult"/>.
        /// </returns>
    {:
            (:Space4, new MethodSignatureTemplate("TResult", node.MemberNames().GenericVisitMethod, node, true):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct GenericVisitorInterfaceTemplate(BaseNodeModel model);
