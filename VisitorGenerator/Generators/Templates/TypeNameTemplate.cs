// SPDX-License-Identifier: MPL-2.0

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
