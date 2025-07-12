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
    /// <inheritdoc/>
    public override (:model.FullName():) Accept(
        (:model.TypeNames().RewriterInterfaceFull:) rewriter, 
        global::System.Threading.CancellationToken cancellationToken = default) 
        => rewriter.(:model.MemberNames().RewriteMethod:)(this, cancellationToken);
    {:
        }
        else
        {
    :}
    /// <inheritdoc/>
    public abstract override (:model.FullName():) Accept(
        (:model.TypeNames().RewriterInterfaceFull:) rewriter, 
        global::System.Threading.CancellationToken cancellationToken = default);
    {:
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct NodeBodyTemplate(NodeModel model);
