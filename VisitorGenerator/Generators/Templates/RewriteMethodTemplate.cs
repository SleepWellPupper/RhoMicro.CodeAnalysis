namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <inheritdoc/>
    public virtual (:model.FullName():) (:model.MemberNames().RewriteMethod:)(
        (:model.FullName():) target,
        global::System.Threading.CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return target;
    }

    """
    )]
[NonEquatable]
internal readonly partial struct RewriteMethodTemplate(NodeModel model);
