// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    (:prefix:)(:model.Name:){:
        if(model.TypeParameters is [_,..])
        {
            (:'<':)
    
            for(var i = 0; i < model.TypeParameters.Count; i++)
            {
                if(i > 0)
                {
                    (:", ":)
                }
                
                (:model.TypeParameters[i]:)
            }   

            (:'>':)
        }
    :}
    """)]
[NonEquatable]
internal readonly partial struct MethodNameTemplate(String prefix,NodeSignatureModel model);
