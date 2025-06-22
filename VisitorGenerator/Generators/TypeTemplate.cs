namespace RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

[Template(
    """
    partial (:model.Flags.HasFlag(NodeSignatureFlags.IsRecord) ? "record" : "class":) (:model.Name:){:
        if(model.TypeParameters is [_,..] typeParameters)    
        {
            (:'<':)
    
            for(var i = 0; i < typeParameters.Count; i++)
            {
                if(i > 0)
                {
                    :}, {:
                }
    
                (:typeParameters[i]:)
            }
    
            (:'>':)
        }
    :}

    {
    (:Space4, body:)
    }
    """)]
[NonEquatable]
internal readonly partial struct TypeTemplate<TBody>(NodeSignatureModel model, TBody body)
    where TBody : ITemplate;
