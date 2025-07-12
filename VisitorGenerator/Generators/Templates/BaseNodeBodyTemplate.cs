// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Invokes the visitor method corresponding to the type of this target (double dispatch).
    /// </summary>
    /// <param name="visitor">
    /// The visitor whose visit method to invoke.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request dispatching to be cancelled.
    /// </param>
    public abstract void Accept(
        (:model.TypeNames().VisitorInterfaceFull:) visitor,
        global::System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Invokes the visitor method corresponding to the type of this target (double dispatch),
    /// producing some result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result produced by the visitor.
    /// </typeparam>
    /// <param name="visitor">
    /// The visitor whose visit method to invoke.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used to request dispatching to be cancelled.
    /// </param>
    /// <returns>
    /// The result produced by dispatching to the visitor.
    /// </returns>
    public abstract TResult Accept<TResult>(
        (:model.TypeNames().GenericVisitorInterfaceFull:) visitor,
        global::System.Threading.CancellationToken cancellationToken = default);
    """)]
[NonEquatable]
internal readonly partial struct BaseNodeBodyTemplate(BaseNodeModel model);
