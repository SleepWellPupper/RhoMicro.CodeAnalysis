// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    {:
        if(model.IsLeaf())
        {
    :}
    /// <inheritdoc/>
    public override void Accept(
        (:model.TypeNames().VisitorInterfaceFull:) visitor,
        global::System.Threading.CancellationToken cancellationToken = default)
        => visitor.(:model.MemberNames().VisitMethod:)(this, cancellationToken);
    /// <inheritdoc/>
    public override TResult Accept<TResult>(
        (:model.TypeNames().GenericVisitorInterfaceFull:) visitor,
        global::System.Threading.CancellationToken cancellationToken = default)
        => visitor.(:model.MemberNames().VisitMethod:)(this, cancellationToken);
    {:
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct NodeBodyTemplate(NodeModel model);
