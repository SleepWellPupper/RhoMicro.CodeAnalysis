namespace RhoMicro.CodeAnalysis;

[Template(
    """
    (:new NamespaceTemplate<TypeTemplate<BaseNodeBodyTemplate>>(model.Signature, new TypeTemplate<BaseNodeBodyTemplate>(model.Signature, new BaseNodeBodyTemplate(model))):)
    (:new NamespaceTemplate<VisitorInterfaceTemplate>(model.Signature, new VisitorInterfaceTemplate(model)):)
    (:new NamespaceTemplate<VisitorClassTemplate>(model.Signature, new VisitorClassTemplate(model)):)
    (:new NamespaceTemplate<GenericVisitorInterfaceTemplate>(model.Signature, new GenericVisitorInterfaceTemplate(model)):)
    (:new NamespaceTemplate<GenericVisitorClassTemplate>(model.Signature, new GenericVisitorClassTemplate(model)):)
    {:
        foreach(var node in model.Nodes)
        {
            (:new NodeTemplate(node):)
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct BaseNodeTemplate(BaseNodeModel model);
