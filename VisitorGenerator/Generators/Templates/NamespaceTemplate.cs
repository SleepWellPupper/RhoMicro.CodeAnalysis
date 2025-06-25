namespace RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

[Template(
    """
    {:
        if(model.Namespace is not [_,..])
        {
    :}
    (:typeBody:)
    {:
            return;
        }
    :}
    namespace (:model.Namespace:)
    {
    (:Space4, typeBody:)
    }


    """)]
[NonEquatable]
internal readonly partial struct NamespaceTemplate<TBody>(NodeSignatureModel model, TBody typeBody)
    where TBody : ITemplate;
