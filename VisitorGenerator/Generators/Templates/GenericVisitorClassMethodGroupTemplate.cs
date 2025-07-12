// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <inheritdoc/>
    public virtual (:new MethodSignatureTemplate("TResult", model.MemberNames().GenericVisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = GetDefault();
        var tmp = result;

        tmp = (:model.MemberNames().GenericOnBeforeVisitMethod:)(target, cancellationToken);
        result = Aggregate(result, tmp);

        tmp = (:model.MemberNames().GenericTraverseMethod:)(target, cancellationToken);
        result = Aggregate(result, tmp);

        tmp = (:model.MemberNames().GenericOnAfterVisitMethod:)(target, cancellationToken);
        result = Aggregate(result, tmp);

        return result;
    }
    /// <summary>
    /// Template method that is invoked before traversing nodes of type <see cref="(:model.FullName():)"/>.
    /// </summary>
    /// <param name="target">
    /// The node to be traversed after invoking this method.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request execution of the template method to be cancelled.
    /// </param>
    /// <returns>
    /// The result of handling the node before traversal.
    /// </returns>
    public virtual (:new MethodSignatureTemplate("TResult", model.MemberNames().GenericOnBeforeVisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = GetDefault();

        return result;
    }
    /// <summary>
    /// Traverses a node of type <see cref="(:model.FullName():)"/>.
    /// </summary>
    /// <param name="target">
    /// The node to traverse.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request traversal to be cancelled.
    /// </param>
    /// <remarks>
    /// The order in which child nodes are traversed is not defined.
    /// In addition, no effort is made to avoid cyclical traversal.
    /// Special traversal logic may be implemented by overriding this method.
    /// </remarks>
    /// <returns>
    /// The result of traversing the node.
    /// </returns>
    public virtual (:new MethodSignatureTemplate("TResult", model.MemberNames().GenericTraverseMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = GetDefault();
        var tmp = result;

    {:
            foreach(var property in model.Properties)
            {
                var isNullable = property.Flags.HasFlag(PropertyFlags.IsNullable);

                if(isNullable)
                {
    :}
        if(target.(:property.Name:) is not null)
        {
    {:
                    renderer.Indent(Space4);
                }

                (:Space4, new GenericPropertyTraversalTemplate(property):)

                if(isNullable)
                {
                    renderer.Detent(Space4.Length);
    :}
        }
    {:
                }
            }
        :}

        return result;
    }
    /// <summary>
    /// Template method that is invoked after traversing nodes of type <see cref="(:model.FullName():)"/>.
    /// </summary>
    /// <param name="target">
    /// The node that was traversed before invoking this method.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request execution of the template method to be cancelled.
    /// </param>
    /// <returns>
    /// The result of handling the node after traversal.
    /// </returns>
    public virtual (:new MethodSignatureTemplate("TResult", model.MemberNames().GenericOnAfterVisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = GetDefault();

        return result;
    }


    """, RendererParameterName = "renderer")]
[NonEquatable]
internal readonly partial struct GenericVisitorClassMethodGroupTemplate(NodeModel model);
