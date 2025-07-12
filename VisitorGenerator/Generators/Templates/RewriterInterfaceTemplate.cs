// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    /// <summary>
    /// Provides base abstraction of a rewriter over nodes 
    /// as defined by (:model.Cref():).
    /// </summary>
    (:model.Signature.Flags.HasFlag(NodeSignatureFlags.IsPublic) ? "public" : "internal":) interface (:model.TypeNames().RewriterInterface:)
    {
    {:
        foreach(var node in model.LeafNodes())
        {
    :}
        /// <summary>
        /// Rewrites a node of type <see cref="(:node.FullName():)"/>.
        /// </summary>
        /// <param name="target">
        /// The node to rewrite.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token used to request rewriting to be cancelled.
        /// </param>
    {:

            (:Space4, new MethodSignatureTemplate(node.MemberNames().RewriteMethod, node, true):)
        }
    :}
    }
    """)]
[NonEquatable]
internal readonly partial struct RewriterInterfaceTemplate(BaseNodeModel model);
