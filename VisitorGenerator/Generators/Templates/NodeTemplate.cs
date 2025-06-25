namespace RhoMicro.CodeAnalysis;

[Template("(:new NamespaceTemplate<TypeTemplate<NodeBodyTemplate>>(model.Signature, new TypeTemplate<NodeBodyTemplate>(model.Signature, new NodeBodyTemplate(model))):)")]
[NonEquatable]
internal readonly partial struct NodeTemplate(NodeModel model);
