// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <inheritdoc/>
    public virtual (:new MethodSignatureTemplate("void", model.MemberNames().VisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (:model.MemberNames().OnBeforeVisitMethod:)(target, cancellationToken);
        (:model.MemberNames().TraverseMethod:)(target, cancellationToken);
        (:model.MemberNames().OnAfterVisitMethod:)(target, cancellationToken);
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
    public virtual (:new MethodSignatureTemplate("void", model.MemberNames().OnBeforeVisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();
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
    public virtual (:new MethodSignatureTemplate("void", model.MemberNames().TraverseMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();

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

                (:Space4, new PropertyTraversalTemplate(property):)

                if(isNullable)
                {
                    renderer.Detent(Space4.Length);
    :}
        }
    {:
                }
            }
        :}
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
    public virtual (:new MethodSignatureTemplate("void", model.MemberNames().OnAfterVisitMethod, model):)
    {
        cancellationToken.ThrowIfCancellationRequested();
    }


    """, RendererParameterName = "renderer")]
[NonEquatable]
internal readonly partial struct VisitorClassMethodGroupTemplate(NodeModel model);
