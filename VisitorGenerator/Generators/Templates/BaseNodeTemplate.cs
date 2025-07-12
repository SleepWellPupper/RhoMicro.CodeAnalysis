// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    (:new NamespaceTemplate<VisitorInterfaceTemplate>(model.Signature, new VisitorInterfaceTemplate(model)):)
    (:new NamespaceTemplate<VisitorClassTemplate>(model.Signature, new VisitorClassTemplate(model)):)
    (:new NamespaceTemplate<GenericVisitorInterfaceTemplate>(model.Signature, new GenericVisitorInterfaceTemplate(model)):)
    (:new NamespaceTemplate<GenericVisitorClassTemplate>(model.Signature, new GenericVisitorClassTemplate(model)):)
    (:new NamespaceTemplate<TypeTemplate<BaseNodeBodyTemplate>>(model.Signature, new TypeTemplate<BaseNodeBodyTemplate>(model.Signature, new BaseNodeBodyTemplate(model))):)
    {:
        foreach(var node in model.Nodes)
        {
            (:new NodeTemplate(node):)
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct BaseNodeTemplate(BaseNodeModel model);
