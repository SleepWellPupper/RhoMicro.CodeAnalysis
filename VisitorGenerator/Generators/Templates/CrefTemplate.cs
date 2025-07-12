// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

[Template(
    """
    <see cref="global::{:
        if(model.Namespace is [_,..] @namespace)
        {
            (:@namespace:)(:'.':)
        }
    :}(:model.Name:){:
        if(model.TypeParameters is [_,..] typeParameters)
        {
            (:'{':)

            for(var i = 0; i < typeParameters.Count; i++)
            {
                if(i > 0)
                {
                    :}, {:
                }

                (:typeParameters[i]:)
            }
    
            (:'}':)
        }
    :}(:suffix:)"/>
    """)]
[NonEquatable]
internal readonly partial struct CrefTemplate(
    NodeSignatureModel model,
    String suffix);
