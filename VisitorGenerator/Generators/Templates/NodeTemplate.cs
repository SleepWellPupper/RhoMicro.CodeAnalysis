// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template("(:new NamespaceTemplate<TypeTemplate<NodeBodyTemplate>>(model.Signature, new TypeTemplate<NodeBodyTemplate>(model.Signature, new NodeBodyTemplate(model))):)")]
[NonEquatable]
internal readonly partial struct NodeTemplate(NodeModel model);
