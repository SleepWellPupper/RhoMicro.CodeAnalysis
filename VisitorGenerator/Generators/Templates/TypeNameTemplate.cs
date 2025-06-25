namespace RhoMicro.CodeAnalysis;

[Template(
    """
    {:
        if(full)
        {
            :}global::{:
            if(model.Namespace is [_,..] @namespace)
            {
                (:@namespace:)(:'.':)
            }
        }
    :}(:prefix:)(:model.Name:)(:suffix:)
    """)]
[NonEquatable]
internal readonly partial struct TypeNameTemplate(
    String prefix,
    NodeSignatureModel model,
    String suffix,
    Boolean full);
