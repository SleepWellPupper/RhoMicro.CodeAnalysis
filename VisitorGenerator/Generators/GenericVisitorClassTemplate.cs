namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Implements the visitor pattern as defined by <see cref="(:model.TypeNames().VisitorInterfaceFull:){TResult}"/>.
    /// </summary>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) abstract class (:model.TypeNames().GenericVisitorClass:) : (:model.TypeNames().GenericVisitorInterfaceFull:)
    {
        /// <summary>
        /// Aggregates two results into one.
        /// </summary>
        /// <param name="first">
        /// The first result to aggregate.
        /// </param>
        /// <param name="second">
        /// The second result to aggregate.
        /// </param>
        /// <returns>
        /// The aggregate of <paramref name="first"/> and <paramref name="second"/>.
        /// </returns>
        protected virtual TResult Aggregate(TResult? first, TResult? second) => first ?? second ?? GetDefault();
        /// <summary>
        /// Gets the default value to use for <typeparamref name="TResult"/>.
        /// </summary>
        /// <returns>
        /// The default value for <typeparamref name="TResult"/>.
        /// </returns>
        protected abstract TResult GetDefault();

    {:
        foreach(var node in model.Nodes)
        {
            (:Space4, new GenericVisitorClassMethodGroupTemplate(node):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct GenericVisitorClassTemplate(BaseNodeModel model);
